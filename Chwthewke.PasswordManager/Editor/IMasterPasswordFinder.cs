using System;
using System.Security;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IMasterPasswordFinder
    {
        Guid? IdentifyMasterPassword( SecureString masterPassword );
    }
}