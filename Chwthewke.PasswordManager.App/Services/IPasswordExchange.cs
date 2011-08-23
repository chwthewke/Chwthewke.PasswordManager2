using System.IO;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPasswordExchange
    {
        void ImportPasswords( FileInfo externalPasswordFile );

        void ExportPasswords( FileInfo targetFile );
    }
}