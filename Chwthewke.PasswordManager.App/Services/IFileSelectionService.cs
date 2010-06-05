using System.IO;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IFileSelectionService {
        FileInfo SelectFile( DirectoryInfo initialDirectory );
    }
}