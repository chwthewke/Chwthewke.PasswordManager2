using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.App.Services;
using Microsoft.Win32;
using System.Linq;

namespace Chwthewke.PasswordManager.App.View
{
    public class DialogFileSelectionService : IFileSelectionService
    {
        public FileInfo SelectExternalPasswordFile( DirectoryInfo initialDirectory )
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

        public IEnumerable<FileInfo> SelectExternalPasswordFileToImport( DirectoryInfo initialDirectory )
        {
            OpenFileDialog dialog = new OpenFileDialog
                                        {
                                            Filter = "XML files|*.xml|All files|*.*",
                                            FilterIndex = 1,
                                            Title = "Import password database file...",
                                            InitialDirectory = initialDirectory.FullName,
                                            Multiselect = true
                                        };
            bool? result = dialog.ShowDialog( );
            if ( result.Value )
                return dialog.FileNames.Select( fn => new FileInfo( fn ) );
            return null;
        }
    }
}