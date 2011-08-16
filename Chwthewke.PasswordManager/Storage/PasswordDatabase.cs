﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    class PasswordDatabase : IPasswordDatabase
    {

        public PasswordDatabase( IPasswordSerializer passwordSerializer )
        {
            _passwordSerializer = passwordSerializer;
        }

        public IPasswordStore Source
        {
            get { return _source; }
            set
            {
                MergeFromSource( );
                _source = value;
                MergeFromSource( );
                Save( );
            }
        }

        public IList<PasswordDigest> Passwords
        {
            get { return _passwords.Values.ToList( ); }
        }

        public PasswordDigest FindByKey(string key)
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
            // Note conflicting modification check possible
            Add( password );
            Save( );
        }

        public void Remove( string key )
        {
            MergeFromSource( );
            // Note conflicting modification check possible
            if (_passwords.Remove( key ))
                Save( );
        }

        private void MergeFromSource( )
        {
            foreach( PasswordDigest password in _passwordSerializer.Load( Source ) )
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

        private IPasswordStore _source = new NullPasswordStore( );

        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );
        private readonly IPasswordSerializer _passwordSerializer;
    }

    internal class NullPasswordStore : IPasswordStore
    {
        public IEnumerable<PasswordDigest> Load( )
        {
            throw new NotImplementedException( );
        }

        public void Save( IEnumerable<PasswordDigest> passwords )
        {
            throw new NotImplementedException( );
        }

        public TextReader OpenReader( )
        {
            return TextReader.Null;
        }

        public TextWriter OpenWriter( )
        {
            return TextWriter.Null;
        }
    }


}