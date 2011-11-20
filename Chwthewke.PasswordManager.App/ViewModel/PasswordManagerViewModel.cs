using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordManagerViewModel : ObservableObject
    {
        public PasswordManagerViewModel( IFileSelectionService fileSelectionService,
                                         IDialogService dialogService,
                                         IPasswordExchange passwordExchange,
                                         IStorageConfiguration storageConfiguration,
                                         PasswordListViewModel passwordList )
        {
            _passwordList = passwordList;
            _passwordExchange = passwordExchange;
            _storageConfiguration = storageConfiguration;
            _fileSelectionService = fileSelectionService;
            _dialogService = dialogService;

            _selectInternalStorageCommand = new RelayCommand( ExecuteSelectInternalStorage );
            _selectExternalStorageCommand = new RelayCommand( ExecuteSelectExternalStorage );
            _quitCommand = new RelayCommand( ExecuteQuit );
            _importPasswordsCommand = new RelayCommand( ExecuteImportPasswords );
            _exportPasswordsCommand = new RelayCommand( ExecuteExportPasswords );

            Update( );
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
            try
            {
                _storageConfiguration.SelectInternalStorage( );
            }
            catch ( PasswordsFileException e )
            {
                _dialogService.ShowFileError( string.Format( "Error while switching storage to internal: {0}", e.Message ) );
            }

            Update( );
        }


        private void ExecuteSelectExternalStorage( )
        {
            FileInfo externalFile =
                _fileSelectionService.SelectExternalPasswordFile( _initialDirectory );

            if ( externalFile == null )
                return;

            try
            {
                _storageConfiguration.SelectExternalStorage( externalFile );
            }
            catch ( PasswordsFileException e )
            {
                _dialogService.ShowFileError( string.Format( "Error while switching storage to external file {0}: {1}", externalFile, e.Message ) );
            }

            Update( );
        }


        private void Update( )
        {
            ExternalStorageSelected = _storageConfiguration.StorageType == StorageType.External;
            InternalStorageSelected = _storageConfiguration.StorageType == StorageType.Internal;

            _passwordList.UpdateList( );
        }

        private void ExecuteImportPasswords( )
        {
            foreach ( FileInfo importedFile in 
                _fileSelectionService.SelectExternalPasswordFileToImport( _initialDirectory ) )
            {
                try
                {
                    _passwordExchange.ImportPasswords( importedFile );
                }
                catch ( PasswordsFileException e )
                {
                    _dialogService.ShowFileError( string.Format( "Error while importing from {0}: {1}", importedFile.Name, e.Message ) );
                }
            }

            _passwordList.UpdateList( );
        }

        private void ExecuteExportPasswords( )
        {
            FileInfo targetFile = _fileSelectionService.SelectExternalPasswordFile( _initialDirectory );

            if ( targetFile == null )
                return;

            try
            {
                _passwordExchange.ExportPasswords( targetFile );
            }
            catch ( PasswordsFileException e )
            {
                _dialogService.ShowFileError( string.Format( "Error while exporting to {0}: {1}", targetFile.Name, e.Message ) );
            }
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

        private readonly PasswordListViewModel _passwordList;
        private readonly IFileSelectionService _fileSelectionService;
        private readonly IDialogService _dialogService;

        private readonly IPasswordExchange _passwordExchange;
        private readonly IStorageConfiguration _storageConfiguration;

        private bool _externalStorageSelected;
        private bool _internalStorageSelected;

        private readonly DirectoryInfo _initialDirectory =
            new DirectoryInfo( Environment.GetFolderPath( Environment.SpecialFolder.Personal ) );
    }
}