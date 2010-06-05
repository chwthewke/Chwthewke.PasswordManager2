using System.IO;
using Microsoft.Win32;

namespace Chwthewke.PasswordManager.App.Services
{
    public class DialogFileSelectionService : IFileSelectionService
    {
        public FileInfo SelectFile( DirectoryInfo initialDirectory )
        {
            SaveFileDialog dialog = new SaveFileDialog
                                        {
                                            AddExtension = true,
                                            DefaultExt = ".xml",
                                            InitialDirectory = initialDirectory.FullName,
                                            Title = "Select password database file",
                                            Filter = "XML files|*.xml",
                                            FilterIndex = 1
                                        };
            bool? result = dialog.ShowDialog( );
            if ( result.Value )
                return new FileInfo( dialog.FileName );
            return null;
        }
    }
}