using System;
using Chwthewke.MvvmUtils;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public string Key
        {
            get { return _key; }
            set
            {
                if ( _key == value )
                    return;
                _key = value;
                RaisePropertyChanged( ( ) => Key );
                Title = Key + "*";
            }
        }

        public string Title
        {
            get { return _title; }
            set
            {
                if ( _title == value )
                    return;
                _title = value;
                RaisePropertyChanged( ( ) => Title );
            }
        }



        private string _key;
        private string _title;

    }
}