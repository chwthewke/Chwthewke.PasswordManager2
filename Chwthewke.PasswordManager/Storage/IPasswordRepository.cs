using System;
using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Storage
{
    [Obsolete]
    public interface IPasswordRepository
    {
        void AddOrUpdate( PasswordDigest passwordDigest );

        bool Remove( string key );

        IEnumerable<PasswordDigest> Passwords { get; }

        PasswordDigest FindPasswordInfo( string key );
    }
}