using System;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordSlotViewModel : ObservableObject
    {
        public PasswordSlotViewModel( IPasswordGenerator generator )
        {
            if ( generator == null )
                throw new ArgumentNullException( "generator" );
            _generator = generator;
        }

        public IPasswordGenerator Generator
        {
            get { return _generator; }
        }

        public string Name
        {
            get { return Resources.ResourceManager.GetString( PasswordGeneratorKeyPrefix + _generator.Id.ToString( "N" ) ); }
        }

        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if ( _isSelected == value )
                    return;
                _isSelected = value;
                RaisePropertyChanged( ( ) => IsSelected );
            }
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
        private bool _isSelected;

        private readonly IPasswordGenerator _generator;

        private const string PasswordGeneratorKeyPrefix = "PasswordGenerator";
    }
}