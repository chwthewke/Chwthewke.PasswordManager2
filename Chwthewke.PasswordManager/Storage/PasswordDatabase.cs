﻿using System;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordDatabase : IPasswordDatabase
    {
        public PasswordDatabase( IPasswordSerializer passwordSerializer, Func<IPasswordStore> sourceProvider )
        {
            _passwordSerializer = passwordSerializer;
            _source = sourceProvider( );
            Init( );
        }

        public IPasswordStore Source
        {
            get { return _source; }
            set
            {
                MergeFromSource( );
                _source = value;
                Save( );
            }
        }

        public IList<PasswordDigest> Passwords
        {
            get { return _passwords.Values.ToList( ); }
        }

        public PasswordDigest FindByKey( string key )
        {
            return _passwords.ContainsKey( key ) ? _passwords[ key ] : null;
        }

        public void Reload( )
        {
            MergeFromSource( );
        }

        public void AddOrUpdate( PasswordDigest password )
        {
            MergeFromSource( );

            Add( password );
            Save( );
        }

        public void Remove( string key )
        {
            if ( !_passwords.ContainsKey( key ) )
                return;

            DateTime localModification = _passwords[ key ].ModificationTime;

            MergeFromSource( );

            if ( _passwords.ContainsKey( key ) && _passwords[ key ].ModificationTime.CompareTo( localModification ) <= 0 )
                _passwords.Remove( key );

            Save( );
        }

        private void Init( )
        {
            MergeFromSource( );
        }

        private void MergeFromSource( )
        {
            foreach ( PasswordDigest password in _passwordSerializer.Load( Source ) )
            {
                PasswordDigest local = FindByKey( password.Key );
                if ( local == null || local.ModificationTime.CompareTo( password.ModificationTime ) < 0 )
                    Add( password );
            }
        }

        private void Add( PasswordDigest password )
        {
            _passwords[ password.Key ] = password;
        }

        private void Save( )
        {
            _passwordSerializer.Save( _passwords.Values, Source );
        }

        private IPasswordStore _source;

        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );
        private readonly IPasswordSerializer _passwordSerializer;
    }
}