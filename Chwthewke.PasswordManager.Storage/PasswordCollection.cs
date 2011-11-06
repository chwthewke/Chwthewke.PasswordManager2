using System;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordCollection : IPasswordCollection
    {
        public PasswordCollection( IPasswordData passwordData )
        {
            _passwordData = passwordData;
        }

        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            return _passwordData.LoadPasswords( ).ToList( );
        }

        public PasswordDigestDocument LoadPassword( string key )
        {
            IEnumerable<PasswordDigestDocument> passwords = LoadPasswords( );
            return FindPassword( key, passwords );
        }


        public bool UpdatePassword( PasswordDigestDocument original, PasswordDigestDocument password )
        {

            throw new NotImplementedException( );
        }

        public bool SavePassword( PasswordDigestDocument password )
        {
            throw new NotImplementedException( );
        }

        public bool DeletePassword( PasswordDigestDocument password )
        {

            throw new NotImplementedException( );
        }

        private PasswordDigestDocument FindPassword( string key, IEnumerable<PasswordDigestDocument> passwords )
        {
            return passwords.FirstOrDefault( d => d.Digest.Key == key );
        }

        private readonly IPasswordData _passwordData;
    }
}