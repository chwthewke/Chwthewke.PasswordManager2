using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using System.Windows;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Migration.Properties;
using Microsoft.Win32;
using System.Linq;

namespace Chwthewke.PasswordManager.Migration
{
    public class ImporterViewModel : ObservableObject
    {
        public ImporterViewModel( ILegacyItemLoader loader, ILegacyItemImporter importer )
        {
            _importCommand = new RelayCommand( Import, CanImport );
            _browseSettingsCommand = new RelayCommand( BrowseSourceFile );
            _saveCommand = new RelayCommand( Save, CanSave );
            _saveToSettingsCommand = new RelayCommand( SaveToSettings, CanSave );

            _loader = loader;
            _importer = importer;

            PasswordsTooltip = MakePasswordsTooltip( );
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
                _importCommand.RaiseCanExecuteChanged( );
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
                _saveCommand.RaiseCanExecuteChanged( );
                _saveToSettingsCommand.RaiseCanExecuteChanged(  );
            }
        }

        private string _passwordsTooltip;

        public string PasswordsTooltip
        {
            get { return _passwordsTooltip; }
            set
            {
                if ( _passwordsTooltip == value )
                    return;
                _passwordsTooltip = value;
                RaisePropertyChanged( ( ) => PasswordsTooltip );
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

        public ICommand SaveToSettingsCommand
        {
            get { return _saveToSettingsCommand; }
        }

        public void UpdateMasterPassword( SecureString masterPassword )
        {
            _masterPassword = masterPassword;
            _importCommand.RaiseCanExecuteChanged( );
        }

        private bool CanImport( )
        {
            return CheckSourceFile( ) && _masterPassword.Length > 0;
        }

        private void Import( )
        {
            if ( !CanImport( ) )
                return;

            try
            {
                IEnumerable<LegacyItem> items = _loader.Load( File.OpenText( SourceFile ) );
                _importer.Import( items, _masterPassword );

                NumPasswords = _importer.NumPasswords;
                PasswordsTooltip = MakePasswordsTooltip( );
            }
            catch ( Exception e )
            {
                ShowException( e );
            }
        }

        private string MakePasswordsTooltip( )
        {
            if ( _importer.NumPasswords == 0 )
                return "Nothing imported yet.";

            return _importer.PasswordKeys.Aggregate( ( p, k ) =>
                                                         {
                                                             int lineLength = p.Length - p.LastIndexOf( '\n' );
                                                             string pad = lineLength > 120 ? ",\n" : ", ";
                                                             return p + pad + k;
                                                         } );
        }

        private void ShowException( Exception e )
        {
            MessageBox.Show( e.Message, "Error", MessageBoxButton.OK, MessageBoxImage.Error );
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
                                            CustomPlaces = SettingsCustomPlaces,
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
            SaveFileDialog dialog = new SaveFileDialog
                                        {
                                            CustomPlaces = SettingsCustomPlaces,
                                            InitialDirectory = FileDialogCustomPlaces.Desktop.Path,
                                            DefaultExt = ".pmd"
                                        };

            bool? result = dialog.ShowDialog( );
            if ( !result.Value )
                return;

            try
            {
                _importer.Save( dialog.FileName );
            }
            catch ( Exception e )
            {
                ShowException( e );
            }
        }

        private void SaveToSettings( )
        {
            Settings.Default.PasswordDatabase = _importer.ToString( );
            Settings.Default.Save( );
        }

        private string _sourceFile;
        private int _numPasswords;

        private readonly IUpdatableCommand _importCommand;
        private readonly IUpdatableCommand _browseSettingsCommand;
        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _saveToSettingsCommand;

        private readonly ILegacyItemLoader _loader;
        private readonly ILegacyItemImporter _importer;

        private static readonly IList<FileDialogCustomPlace> SettingsCustomPlaces =
            new List<FileDialogCustomPlace>
                {
                    FileDialogCustomPlaces.Desktop,
                    FileDialogCustomPlaces.Documents,
                    FileDialogCustomPlaces.LocalApplicationData,
                };

        private SecureString _masterPassword = new SecureString( );
    }
}