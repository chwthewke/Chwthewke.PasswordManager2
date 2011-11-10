using System;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordSlotViewModel2 : ObservableObject
    {
        public PasswordSlotViewModel2( IDerivedPasswordModel model, IPasswordEditorModel parent )
        {
            _model = model;
            _parent = parent;
        }

        public string GeneratorDescription
        {
            get { return Resources.ResourceManager.GetString( PasswordGeneratorTranslator.DescriptionKey( Generator ) ); }
        }


        public string Name
        {
            get { return Resources.ResourceManager.GetString( PasswordGeneratorTranslator.NameKey( Generator ) ); }
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

        public void Update( )
        {
            Content = _model.DerivedPassword.Password;
            IsSelected = _parent.SelectedPassword == _model;
        }

        private Guid Generator
        {
            get { return _model.Generator; }
        }

        private string _content = string.Empty;
        private bool _isSelected;

        private readonly IPasswordEditorModel _parent;
        private readonly IDerivedPasswordModel _model;
    }
}