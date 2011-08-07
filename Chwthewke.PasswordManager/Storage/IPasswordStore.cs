using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore {
        IEnumerable<PasswordDigest> Load( );
        void Save( IEnumerable<PasswordDigest> passwords );
    }
}