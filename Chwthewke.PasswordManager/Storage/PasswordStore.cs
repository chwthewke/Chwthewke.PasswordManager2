using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordStore : IPasswordStore
    {
        public PasswordInfo AddOrUpdate( PasswordInfo passwordInfo )
        {
            PasswordInfo result = FindPasswordInfo( passwordInfo.Key );

            _passwords[ passwordInfo.Key ] = passwordInfo;

            return result;
        }

        public bool Remove( PasswordInfo passwordInfo )
        {
            throw new NotImplementedException( );
        }

        public IEnumerable<PasswordInfo> Passwords
        {
            get { return new List<PasswordInfo>( _passwords.Values ); }
        }

        public PasswordInfo FindPasswordInfo( string key )
        {
            PasswordInfo result;
            _passwords.TryGetValue( key, out result );
            return result;
        }

        private readonly IDictionary<string, PasswordInfo> _passwords = new Dictionary<string, PasswordInfo>( );
    }
}