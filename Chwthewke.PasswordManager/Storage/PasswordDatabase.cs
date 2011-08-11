using System;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    class PasswordDatabase : IPasswordDatabase
    {

        public PasswordDatabase( IPasswordSerializer passwordSerializer, Func<IPasswordStore> sourceProvider )
        {
            _passwordSerializer = passwordSerializer;
            Source = sourceProvider();
            Init( );
        }

        public IPasswordStore Source { get; set; }

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

        private void Init( )
        {
            MergeFromSource( );
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


        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );
        private readonly IPasswordSerializer _passwordSerializer;
    }


}