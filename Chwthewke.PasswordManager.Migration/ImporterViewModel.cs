using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Microsoft.Win32;

namespace Chwthewke.PasswordManager.Migration
{
    public class ImporterViewModel : ObservableObject
    {
        public ImporterViewModel( LegacyItemLoader loader, LegacyItemImporter importer )
        {
            _importCommand = new RelayCommand<SecureString>( Import, CanImport );
            _browseSettingsCommand = new RelayCommand( BrowseSourceFile );
            _saveCommand = new RelayCommand( Save, CanSave );

            _loader = loader;
            _importer = importer;
        }

        public string SourceFile
        {
            get { return _sourceFile; }
            set
            {
                if ( _sourceFile == value )
                    return;
                _sourceFile = value;
                RaisePropertyChanged( ( ) => SourceFile );
            }
        }

        public int NumPasswords
        {
            get { return _numPasswords; }
            set
            {
                if ( _numPasswords == value )
                    return;
                _numPasswords = value;
                RaisePropertyChanged( ( ) => NumPasswords );
            }
        }

        public ICommand ImportCommand
        {
            get { return _importCommand; }
        }

        public ICommand BrowseSettingsCommand
        {
            get { return _browseSettingsCommand; }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        private bool CanImport( SecureString masterPassword )
        {
            return CheckSourceFile( ) && masterPassword.Length > 0;
        }

        private void Import( SecureString masterPassword )
        {
            throw new NotImplementedException( );
        }

        private bool CheckSourceFile( )
        {
            if ( string.IsNullOrWhiteSpace( SourceFile ) )
                return false;
            if ( !File.Exists( SourceFile ) )
                return false;
            return true;
        }

        private void BrowseSourceFile( )
        {
            OpenFileDialog dialog = new OpenFileDialog
                                        {
                                            CustomPlaces = _settingsCustomPlaces,
                                            DereferenceLinks = true,
                                            InitialDirectory = FileDialogCustomPlaces.LocalApplicationData.Path
                                        };
            bool? result = dialog.ShowDialog( );
            if ( !result.Value )
                return;

            SourceFile = dialog.FileName;
        }

        private bool CanSave( )
        {
            return NumPasswords > 0;
        }

        private void Save( )
        {
            throw new NotImplementedException( );
        }

        private string _sourceFile;
        private int _numPasswords;

        private readonly ICommand _importCommand;
        private readonly ICommand _browseSettingsCommand;
        private readonly ICommand _saveCommand;

        private readonly LegacyItemLoader _loader;
        private readonly LegacyItemImporter _importer;

        private static readonly IList<FileDialogCustomPlace> _settingsCustomPlaces =
            new List<FileDialogCustomPlace>
                {
                    FileDialogCustomPlaces.Desktop,
                    FileDialogCustomPlaces.Documents,
                    FileDialogCustomPlaces.LocalApplicationData,
                };
    }
}