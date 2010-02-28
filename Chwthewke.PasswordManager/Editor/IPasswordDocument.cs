using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordDocument
    {
        string Key { get; set; }

        string GeneratedPassword( IPasswordFactory factory );

        PasswordInfo SavedPasswordInfo { get; }
    }
}