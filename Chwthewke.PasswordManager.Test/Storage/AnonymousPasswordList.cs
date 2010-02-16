using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Storage.Serialization;

namespace Chwthewke.PasswordManager.Tests.Storage
{
    internal class AnonymousPasswordList : IPasswordStorageService
    {
        public IEnumerable<string> KnownKeys { get { return Enumerable.Empty<string>( ); } }

        public bool HasPassword( string key )
        {
            throw new NotImplementedException( );
        }

        public bool ValidatePassword( string key, string password, PasswordType type )
        {
            throw new NotImplementedException( );
        }

        public void SetPassword( string key, string password, PasswordType type )
        {
            throw new NotImplementedException( );
        }

        public void ClearPassword( string key )
        {
            throw new NotImplementedException( );
        }

        public ISerializer Serializer { get { throw new NotImplementedException( ); } }
    }
}