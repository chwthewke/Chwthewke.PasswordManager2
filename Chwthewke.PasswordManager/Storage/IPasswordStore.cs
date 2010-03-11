using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore
    {
        void AddOrUpdate( PasswordDigest passwordDigest );

        bool Remove( PasswordDigest passwordDigest );

        IEnumerable<PasswordDigest> Passwords { get; }

        PasswordDigest FindPasswordInfo( string key );
    }
}