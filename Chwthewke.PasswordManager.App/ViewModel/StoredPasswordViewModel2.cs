using System;
using System.Windows.Media;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class StoredPasswordViewModel2 : ObservableObject
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
            get { return _fuzzyDateFormatter.Format( _password.CreatedOn ); }
        }

        public string ModificationDate
        {
            get { return _fuzzyDateFormatter.Format( _password.ModifiedOn ); }
        }

        public string GeneratorName
        {
            get
            {
                return Resources.ResourceManager.GetString(
                    PasswordGeneratorTranslator.NameKey( _password.PasswordGenerator ) );
            }
        }

        public PasswordDigestDocument PasswordDocument
        {
            get { return _password; }
        }

        public StoredPasswordViewModel2( PasswordDigestDocument password, IGuidToColorConverter guidColorConverter, IFuzzyDateFormatter fuzzyDateFormatter )
        {
            _password = password;
            _guidColorConverter = guidColorConverter;
            _fuzzyDateFormatter = fuzzyDateFormatter;
        }

        public delegate StoredPasswordViewModel2 Factory( PasswordDigestDocument password );

        private readonly PasswordDigestDocument _password;
        private readonly IGuidToColorConverter _guidColorConverter;
        private readonly IFuzzyDateFormatter _fuzzyDateFormatter;
    }
}