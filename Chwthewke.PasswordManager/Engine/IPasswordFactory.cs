using System.Security;

namespace Chwthewke.PasswordManager.Engine
{
    public interface IPasswordFactory
    {
        string MakePassword( string key, SecureString masterPassword );
    }
}