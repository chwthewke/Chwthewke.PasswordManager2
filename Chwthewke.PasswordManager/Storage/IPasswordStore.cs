using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore
    {
        PasswordInfo Add( PasswordInfo passwordInfo );

        bool Remove( PasswordInfo passwordInfo );

        IEnumerable<PasswordInfo> Passwords { get; }

        PasswordInfo FindPasswordInfo( string key );
    }
}