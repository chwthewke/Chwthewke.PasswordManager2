using System;
using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore
    {
        void AddOrUpdate( PasswordDigest passwordDigest );

        bool Remove( string key );

        IEnumerable<PasswordDigest> Passwords { get; }

        PasswordDigest FindPasswordInfo( string key );

        Guid? IdentifyMasterPassword( SecureString masterPassword );

        bool MatchMasterPassword( PasswordDigest dig, SecureString masterPassword );
    }
}