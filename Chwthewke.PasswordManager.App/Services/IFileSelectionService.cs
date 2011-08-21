using System.Collections.Generic;
using System.IO;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IFileSelectionService
    {
        FileInfo SelectExternalPasswordFile( DirectoryInfo initialDirectory );

        IEnumerable<FileInfo> SelectExternalPasswordFileToImport( DirectoryInfo initialDirectory );
    }
}