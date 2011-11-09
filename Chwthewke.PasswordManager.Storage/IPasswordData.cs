using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    internal interface IPasswordData
    {
        IList<PasswordDigestDocument> LoadPasswords( );
        void SavePasswords( IList<PasswordDigestDocument> passwords );
    }
}