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
    public class PasswordManagerViewModel2 : ObservableObject
    {
        public PasswordManagerViewModel2( IPasswordManagerStorage storage,
                                          IFileSelectionService fileSelectionService,
                                          IPasswordExchange passwordExchange,
                                          Settings settings,
                                          PasswordListViewModel2 passwordList )
        {
            _storage = storage;
            _passwordList = passwordList;
            _passwordExchange = passwordExchange;
            _fileSelectionService = fileSelectionService;
            _settings = settings;

            _selectInternalStorageCommand = new RelayCommand( ExecuteSelectInternalStorage );
            _selectExternalStorageCommand = new RelayCommand( ExecuteSelectExternalStorage );
            _quitCommand = new RelayCommand( ExecuteQuit );
            _importPasswordsCommand = new RelayCommand( ExecuteImportPasswords );
            _exportPasswordsCommand = new RelayCommand( ExecuteExportPasswords );

            UpdateStorageType( );
        }

        public PasswordListViewModel2 PasswordList
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

        public ICommand ExportPasswordsCommand
        {
            get { return _exportPasswordsCommand; }
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
            ITextResource passwordSource;
            if ( _settings.PasswordsAreExternal )
                passwordSource = new FileTextResource( new FileInfo( _settings.ExternalPasswordDatabase ) );
            else
                passwordSource = new SettingsTextResource( _settings );
            _storage.PasswordRepository.PasswordData = XmlPasswordData.From( passwordSource );

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
                _passwordExchange.ImportPasswords( importedFile );
            }

            _passwordList.UpdateList( );
        }

        private void ExecuteExportPasswords( )
        {
            FileInfo targetFile = _fileSelectionService.SelectExternalPasswordFile( _initialDirectory );

            if ( targetFile != null )
                _passwordExchange.ExportPasswords( targetFile );
        }


        private static void ExecuteQuit( )
        {
            Application.Current.Shutdown( );
        }

        private readonly ICommand _quitCommand;
        private readonly ICommand _selectInternalStorageCommand;
        private readonly ICommand _selectExternalStorageCommand;
        private readonly ICommand _importPasswordsCommand;
        private readonly ICommand _exportPasswordsCommand;

        private readonly IPasswordManagerStorage _storage;
        private readonly PasswordListViewModel2 _passwordList;
        private readonly IFileSelectionService _fileSelectionService;

        private readonly IPasswordExchange _passwordExchange;
        private readonly Settings _settings;

        private bool _externalStorageSelected;
        private bool _internalStorageSelected;

        private readonly DirectoryInfo _initialDirectory =
            new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
    }
}