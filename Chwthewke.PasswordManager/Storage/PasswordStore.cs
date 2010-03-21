using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordStore : IPasswordStore
    {
        public void AddOrUpdate( PasswordDigest passwordDigest )
        {
            _passwords[ passwordDigest.Key ] = passwordDigest;
        }

        public bool Remove( string key )
        {
            return _passwords.Remove( key );
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