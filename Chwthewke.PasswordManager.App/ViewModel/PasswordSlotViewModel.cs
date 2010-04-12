using System;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordSlotViewModel: ObservableObject {
        public PasswordSlotViewModel( IPasswordGenerator generator )
        {
            _generator = generator;
        }

        public IPasswordGenerator Generator
        {
            get { return _generator; }
        }

        public string Content
        {
            get { return _content; }
            set
            {
                if ( _content == value )
                    return;
                _content = value;
                RaisePropertyChanged( ( ) => Content );
            }
        }

        private string _content = string.Empty;
        private readonly IPasswordGenerator _generator;
    }
}