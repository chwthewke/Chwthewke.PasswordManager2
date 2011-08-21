using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordManagerViewModel : ObservableObject
    {
        public PasswordManagerViewModel( PasswordListViewModel passwordList,
                                         IFileSelectionService fileSelectionService,
                                         IPasswordDatabase passwordDatabase,
                                         Settings settings,
                                         IPasswordImporter passwordImporter )
        {
            _passwordList = passwordList;
            _passwordImporter = passwordImporter;
            _fileSelectionService = fileSelectionService;
            _passwordDatabase = passwordDatabase;
            _settings = settings;

            _selectInternalStorageCommand = new RelayCommand( ExecuteSelectInternalStorage );
            _selectExternalStorageCommand = new RelayCommand( ExecuteSelectExternalStorage );
            _quitCommand = new RelayCommand( ExecuteQuit );
            _importPasswordsCommand = new RelayCommand( ExecuteImportPasswords );

            UpdateStorageType( );
        }

        public PasswordListViewModel PasswordList
        {
            get { return _passwordList; }
        }

        public ICommand SelectInternalStorageCommand
        {
            get { return _selectInternalStorageCommand; }
        }

        public ICommand SelectExternalStorageCommand
        {
            get { return _selectExternalStorageCommand; }
        }

        public ICommand QuitCommand
        {
            get { return _quitCommand; }
        }

        public ICommand ImportPasswordsCommand
        {
            get { return _importPasswordsCommand; }
        }

        public bool InternalStorageSelected
        {
            get { return _internalStorageSelected; }
            set
            {
                if ( _internalStorageSelected == value )
                    return;
                _internalStorageSelected = value;
                RaisePropertyChanged( ( ) => InternalStorageSelected );
            }
        }

        public bool ExternalStorageSelected
        {
            get { return _externalStorageSelected; }
            set
            {
                if ( _externalStorageSelected == value )
                    return;
                _externalStorageSelected = value;
                RaisePropertyChanged( ( ) => ExternalStorageSelected );
            }
        }

        private void ExecuteSelectInternalStorage( )
        {
            SetInternalStorage( );

            UpdateStorageType( );
        }

        private void SetInternalStorage( )
        {
            _settings.PasswordsAreExternal = false;
            _settings.ExternalPasswordDatabase = string.Empty;
            _settings.Save( );
        }

        private void ExecuteSelectExternalStorage( )
        {
            FileInfo externalFile =
                _fileSelectionService.SelectExternalPasswordFile( _initialDirectory );

            if ( externalFile == null )
                return;

            SetExternalStorage( externalFile );

            UpdateStorageType( );
        }

        private void SetExternalStorage( FileInfo externalFile )
        {
            _settings.PasswordsAreExternal = true;
            _settings.ExternalPasswordDatabase = externalFile.FullName;
            _settings.PasswordDatabase = string.Empty;
            _settings.Save( );
        }

        private void UpdateStorageType( )
        {
            UpdateStorageTypeUi( );
            ActivateStorageType( );
        }

        private void ActivateStorageType( )
        {
            if ( _settings.PasswordsAreExternal )
                _passwordDatabase.Source = new FilePasswordStore( new FileInfo( _settings.ExternalPasswordDatabase ) );
            else
                _passwordDatabase.Source = new SettingsPasswordStore( _settings );
            _passwordList.UpdateList( );
        }

        private void UpdateStorageTypeUi( )
        {
            ExternalStorageSelected = _settings.PasswordsAreExternal;
            InternalStorageSelected = !_settings.PasswordsAreExternal;
        }

        private void ExecuteImportPasswords( )
        {
            foreach (
                FileInfo importedFile in _fileSelectionService.SelectExternalPasswordFileToImport( _initialDirectory ) )
            {
                _passwordImporter.ImportPasswords( importedFile );
            }

            _passwordList.UpdateList( );
        }

        private static void ExecuteQuit( )
        {
            Application.Current.Shutdown( );
        }

        private readonly ICommand _quitCommand;
        private readonly ICommand _selectInternalStorageCommand;
        private readonly ICommand _selectExternalStorageCommand;
        private readonly ICommand _importPasswordsCommand;

        private readonly PasswordListViewModel _passwordList;
        private readonly IFileSelectionService _fileSelectionService;

        private readonly IPasswordDatabase _passwordDatabase;

        private readonly IPasswordImporter _passwordImporter;
        private readonly Settings _settings;

        private bool _externalStorageSelected;
        private bool _internalStorageSelected;

        private readonly DirectoryInfo _initialDirectory =
            new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
    }
}