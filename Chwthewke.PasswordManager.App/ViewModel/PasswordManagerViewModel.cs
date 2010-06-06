using System;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordManagerViewModel : ObservableObject
    {
        public PasswordManagerViewModel( PasswordListViewModel passwordList,
                                         IFileSelectionService fileSelectionService,
                                         IPersistenceService persistenceService,
                                         Settings settings )
        {
            _passwordList = passwordList;
            _fileSelectionService = fileSelectionService;
            _persistenceService = persistenceService;
            _settings = settings;

            _selectInternalStorageCommand = new RelayCommand( ExecuteSelectInternalStorage );
            _selectExternalStorageCommand = new RelayCommand( ExecuteSelectExternalStorage );
            _quitCommand = new RelayCommand( ExecuteQuit );

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
            _settings.PasswordsAreExternal = false;
            _settings.ExternalPasswordDatabase = string.Empty;
            _persistenceService.Save( );

            UpdateStorageType( );
        }

        private void ExecuteSelectExternalStorage( )
        {
            FileInfo externalFile =
                _fileSelectionService.SelectExternalPasswordFile( _initialDirectory );

            if ( externalFile == null )
                return;

            _settings.PasswordsAreExternal = true;
            _settings.ExternalPasswordDatabase = externalFile.FullName;
            _settings.PasswordDatabase = string.Empty;
            _persistenceService.Save( );
            _settings.Save( );

            UpdateStorageType( );
        }

        private void UpdateStorageType( )
        {
            ExternalStorageSelected = _settings.PasswordsAreExternal;
            InternalStorageSelected = !_settings.PasswordsAreExternal;
        }

        private void ExecuteImportPasswords( )
        {
            IEnumerable<FileInfo> importedFiles =
                _fileSelectionService.SelectExternalPasswordFileToImport( _initialDirectory );
        }

        private static void ExecuteQuit( )
        {
            Application.Current.Shutdown( );
        }

        private readonly ICommand _quitCommand;
        private readonly ICommand _selectInternalStorageCommand;
        private readonly ICommand _selectExternalStorageCommand;

        private readonly PasswordListViewModel _passwordList;
        private readonly IFileSelectionService _fileSelectionService;
        private readonly IPersistenceService _persistenceService;
        private readonly Settings _settings;

        private bool _externalStorageSelected;
        private bool _internalStorageSelected;

        private readonly DirectoryInfo _initialDirectory =
            new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
    }
}