using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordData
    {
        IList<PasswordDigestDocument> LoadPasswords( );
        void SavePasswords( IList<PasswordDigestDocument> passwords );
    }
}