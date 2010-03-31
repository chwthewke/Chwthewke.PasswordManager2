using System;
using System.Collections.ObjectModel;
using Chwthewke.MvvmUtils;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordGroupingViewModel : ObservableObject
    {
        public string PasswordId
        {
            get { return _passwordId; }
            set
            {
                if ( _passwordId == value )
                    return;
                _passwordId = value;
                RaisePropertyChanged( ( ) => PasswordId );
            }
        }

        private string _passwordId;

        public DateTime CreationTime
        {
            get { return _creationTime; }
            set
            {
                if ( _creationTime == value )
                    return;
                _creationTime = value;
                RaisePropertyChanged( ( ) => CreationTime );
            }
        }

        private DateTime _creationTime;


        public ObservableCollection<string> Passwords
        {
            get { return _passwords; }
            set
            {
                if ( _passwords == value )
                    return;
                _passwords = value;
                RaisePropertyChanged( ( ) => Passwords );
            }
        }

        private ObservableCollection<string> _passwords;
    }
}