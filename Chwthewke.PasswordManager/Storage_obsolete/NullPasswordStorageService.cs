using System;
using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage.Serialization;

namespace Chwthewke.PasswordManager.Storage
{
    public class NullPasswordStorageService : IPasswordStorageService
    {
        public IEnumerable<string> KnownKeys
        {
            get { return Enumerable.Empty<string>( ); }
        }


        public bool HasPassword( string key )
        {
            return false;
        }

        public bool ValidatePassword( string key, string password, PasswordType type )
        {
            return false;
        }

        public void SetPassword( string key, string password, PasswordType type ) {}

        public void ClearPassword( string key ) {}

        public ISerializer Serializer
        {
            get { throw new NotImplementedException( ); }
        }
    }
}