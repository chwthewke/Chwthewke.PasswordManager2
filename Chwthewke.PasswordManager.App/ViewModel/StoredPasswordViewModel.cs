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

        public StoredPasswordViewModel(PasswordDigest password, Color masterPasswordColor)
        {
            _name = password.Key;
            _masterPasswordGuid = password.MasterPasswordId;
            _masterPasswordColor = masterPasswordColor;
            _creationDate = password.CreationTime.ToString( );
            _modificationDate = password.ModificationTime.ToString( );
        }

        private readonly string _name;
        private readonly Guid _masterPasswordGuid;
        private readonly Color _masterPasswordColor;
        private readonly string _creationDate;
        private readonly string _modificationDate;
    }
}