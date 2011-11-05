using System;
using System.Security;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IMasterPasswordMatcher
    {
        Guid? IdentifyMasterPassword( SecureString masterPassword );
    }
}
