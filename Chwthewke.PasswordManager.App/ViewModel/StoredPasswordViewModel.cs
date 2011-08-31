using System;
using System.Windows.Media;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class StoredPasswordViewModel : ObservableObject
    {
        public string Name
        {
            get { return _name; }
        }

        public string Note
        {
            get { return _note; }
        }

        public bool NoteVisible
        {
            get { return !string.IsNullOrWhiteSpace( _note ); }
        }

        public Guid MasterPasswordGuid
        {
            get { return _masterPasswordGuid; }
        }

        public Color MasterPasswordColor
        {
            get { return _masterPasswordColor; }
        }

        public string CreationDate
        {
            get { return _creationDate; }
        }

        public string ModificationDate
        {
            get { return _modificationDate; }
        }

        public string GeneratorName
        {
            get { return _generatorName; }
        }

        public StoredPasswordViewModel(PasswordDigest password, Color masterPasswordColor)
        {
            _name = password.Key;
            _note = password.Note;
            _masterPasswordGuid = password.MasterPasswordId;
            _masterPasswordColor = masterPasswordColor;
            _creationDate = password.CreationTime.ToString( );
            _modificationDate = password.ModificationTime.ToString( );
            _generatorName = PasswordGeneratorNames.GeneratorName( password.PasswordGeneratorId );
        }

        private readonly string _name;
        private readonly string _note;
        private readonly Guid _masterPasswordGuid;
        private readonly Color _masterPasswordColor;
        private readonly string _creationDate;
        private readonly string _modificationDate;
        private readonly string _generatorName;
    }
}