using System;
using System.Windows.Media;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class StoredPasswordViewModel : ObservableObject
    {
        public string Name
        {
            get { return _password.Key; }
        }

        public string Note
        {
            get { return _password.Note; }
        }

        public bool NoteVisible
        {
            get { return !string.IsNullOrWhiteSpace( Note ); }
        }

        public Guid MasterPasswordGuid
        {
            get { return _password.MasterPasswordId; }
        }

        public Color MasterPasswordColor
        {
            get { return _guidColorConverter.Convert( _password.MasterPasswordId ); }
        }

        public string CreationDate
        {
            get { return _fuzzyDateFormatter.Format( _password.CreationTime ); }
        }

        public string ModificationDate
        {
            get { return _fuzzyDateFormatter.Format( _password.ModificationTime ); }
        }

        public string GeneratorName
        {
            get
            {
                return Resources.ResourceManager.GetString(
                    PasswordGeneratorTranslator.NameKey( _password.PasswordGeneratorId ) );
            }
        }

        public StoredPasswordViewModel( PasswordDigest password, IGuidToColorConverter guidColorConverter, IFuzzyDateFormatter fuzzyDateFormatter )
        {
            _password = password;
            _guidColorConverter = guidColorConverter;
            _fuzzyDateFormatter = fuzzyDateFormatter;
        }

        public delegate StoredPasswordViewModel Factory( PasswordDigest password );

        private readonly PasswordDigest _password;
        private readonly IGuidToColorConverter _guidColorConverter;
        private readonly IFuzzyDateFormatter _fuzzyDateFormatter;
    }
}