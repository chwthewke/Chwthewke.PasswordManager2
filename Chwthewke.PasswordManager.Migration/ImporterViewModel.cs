using System;
using System.IO;
using System.Security;
using System.Windows.Input;
using System.Xml.Serialization;
using Chwthewke.MvvmUtils;

namespace Chwthewke.PasswordManager.Migration
{
    public class ImporterViewModel : ObservableObject
    {
        public ImporterViewModel( LegacyItemLoader loader, LegacyItemImporter importer )
        {
            _importCommand = new RelayCommand<SecureString>( Import, CanImport );
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

        public bool HasMasterPassword
        {
            get { return _hasMasterPassword; }
            set
            {
                if ( _hasMasterPassword == value )
                    return;
                _hasMasterPassword = value;
                RaisePropertyChanged( ( ) => HasMasterPassword );
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

        private bool CanImport( SecureString masterPassword )
        {
            return CheckSourceFile( ) && masterPassword.Length > 0;
        }

        private void Import( SecureString masterPassword ) {}

        private bool CheckSourceFile( )
        {
            if ( string.IsNullOrWhiteSpace( SourceFile ) )
                return false;
            if ( !File.Exists( SourceFile ) )
                return false;
            return true;
        }

        private string _sourceFile;
        private bool _hasMasterPassword;
        private int _numPasswords;

        private readonly ICommand _importCommand;

        private readonly LegacyItemLoader _loader;
        private readonly LegacyItemImporter _importer;
    }
}