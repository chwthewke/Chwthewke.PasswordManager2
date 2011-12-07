using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordData
    {
        IEnumerable<PasswordDigestDocument> LoadPasswords( );
        void SavePasswords( IEnumerable<PasswordDigestDocument> passwords );
    }
}