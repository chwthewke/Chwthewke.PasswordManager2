using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore
    {
        void AddOrUpdate( PasswordDigest passwordDigest );

        bool Remove( string key );

        IEnumerable<PasswordDigest> Passwords { get; }

        PasswordDigest FindPasswordInfo( string key );
    }
}