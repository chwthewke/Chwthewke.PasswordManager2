using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage.Serialization;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStorageService
    {
        IEnumerable<string> KnownKeys { get; }

        bool HasPassword( string key );

        bool ValidatePassword( string key, string password, PasswordType type );

        void SetPassword( string key, string password, PasswordType type );

        void ClearPassword( string key );

        ISerializer Serializer { get; }
    }
}