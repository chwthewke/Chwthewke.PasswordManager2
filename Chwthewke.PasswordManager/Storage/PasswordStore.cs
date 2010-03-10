using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordStore : IPasswordStore
    {
        public PasswordDigest AddOrUpdate( PasswordDigest passwordDigest )
        {
            PasswordDigest result = FindPasswordInfo( passwordDigest.Key );

            _passwords[ passwordDigest.Key ] = passwordDigest;

            return result;
        }

        public bool Remove( PasswordDigest passwordDigest )
        {
            return _passwords.Remove( new KeyValuePair<string, PasswordDigest>( passwordDigest.Key, passwordDigest ) );
        }

        public IEnumerable<PasswordDigest> Passwords
        {
            get { return new List<PasswordDigest>( _passwords.Values ); }
        }

        public PasswordDigest FindPasswordInfo( string key )
        {
            PasswordDigest result;
            _passwords.TryGetValue( key, out result );
            return result;
        }

        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );
    }
}