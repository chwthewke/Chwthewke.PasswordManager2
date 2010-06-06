using System.Collections.Generic;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPasswordStore {
        IEnumerable<PasswordDigest> Load( );
        void Save( IEnumerable<PasswordDigest> passwords );
    }
}