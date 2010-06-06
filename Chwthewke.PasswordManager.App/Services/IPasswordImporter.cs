using System.IO;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPasswordImporter {
        void ImportPasswords( FileInfo externalPasswordFile );
    }
}