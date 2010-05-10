using System;
using System.Windows.Media;
using Chwthewke.MvvmUtils;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class StoredPasswordViewModel : ObservableObject
    {
        public string Name
        {
            get { return _name; }
            set
            {
                if ( _name == value )
                    return;
                _name = value;
                RaisePropertyChanged( ( ) => Name );
            }
        }

        public Guid MasterPasswordGuid
        {
            get { return _masterPasswordGuid; }
            set
            {
                if ( _masterPasswordGuid == value )
                    return;
                _masterPasswordGuid = value;
                RaisePropertyChanged( ( ) => MasterPasswordGuid );
            }
        }

        public Color MasterPasswordColor
        {
            get { return _masterPasswordColor; }
            set
            {
                if ( _masterPasswordColor == value )
                    return;
                _masterPasswordColor = value;
                RaisePropertyChanged( ( ) => MasterPasswordColor );
            }
        }

        private string _name;
        private Guid _masterPasswordGuid;
        private Color _masterPasswordColor;


    }
}