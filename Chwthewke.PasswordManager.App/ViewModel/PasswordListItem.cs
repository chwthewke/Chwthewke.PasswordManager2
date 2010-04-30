using System;
using Chwthewke.MvvmUtils;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordListItem : ObservableObject
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


        private string _name;
        private Guid _masterPasswordGuid;



    }
}