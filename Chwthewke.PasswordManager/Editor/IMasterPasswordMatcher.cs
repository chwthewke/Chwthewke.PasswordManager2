using System.Security;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IMasterPasswordMatcher
    {
        bool MatchMasterPassword( SecureString masterPassword, PasswordDigest passwordDigest );
    }
}