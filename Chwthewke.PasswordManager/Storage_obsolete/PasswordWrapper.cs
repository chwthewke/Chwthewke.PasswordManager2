using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordWrapper : IPassword
    {
        private readonly PasswordDictionary _owner;
        private readonly string _key;

        private PasswordData GetOrCreateData( )
        {
            return _owner.GetPasswordData( _key ) ?? new PasswordData( );
        }

        public PasswordWrapper( PasswordDictionary owner, string key )
        {
            _owner = owner;
            _key = key;
        }

        public bool HasPassword
        {
            get { return _owner.GetPasswordData( _key ) != null; }
        }

        public PasswordType PasswordType
        {
            get { return GetOrCreateData( ).Type; }
        }

        public bool ValidatePassword( string generatedPassword )
        {
            byte[ ] storedHash = GetOrCreateData( ).Hash;
            return storedHash != null && PasswordHash( generatedPassword ).SequenceEqual( storedHash );
        }

        public void SetPassword( string generatedPassword, PasswordType passwordType )
        {
            PasswordData data = GetOrCreateData( );
            data.Type = passwordType;
            data.Hash = PasswordHash( generatedPassword );
            _owner.SetPasswordData( _key, data );
        }

        public void Clear( )
        {
            _owner.RemovePasswordData( _key );
        }

        private byte[ ] PasswordHash( string password )
        {
            return new Sha512( ).Hash( Encoding.UTF8.GetBytes( PasswordDictionary.Salt + _key + password ) );
        }
    }
}