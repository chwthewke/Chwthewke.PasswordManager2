using System;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class DerivedPasswordViewModel : ObservableObject
    {
        public DerivedPasswordViewModel( IDerivedPasswordModel model, IPasswordEditorModel parentModel, Settings settings )
        {
            _model = model;
            _parentModel = parentModel;
            _settings = settings;
        }

        public delegate DerivedPasswordViewModel Factory( IDerivedPasswordModel model, IPasswordEditorModel parentModel );

        public string GeneratorDescription
        {
            get { return Resources.ResourceManager.GetString( PasswordGeneratorTranslator.DescriptionKey( Generator ) ); }
        }


        public string Name
        {
            get
            {
                var nameKey = PasswordGeneratorTranslator.NameKey( Generator );
                return Resources.ResourceManager.GetString( nameKey );
            }
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
            Visible = IsSelected || !_model.IsLegacy || _settings.ShowLegacyPasswordGenerators;
        }

        public void Refresh( )
        {
            IsSelected = _parentModel.SelectedPassword == _model;
        }

        public IDerivedPasswordModel Model
        {
            get { return _model; }
        }

        public bool Visible
        {
            get { return _visible; }
            set
            {
                if ( _visible == value )
                    return;
                _visible = value;
                RaisePropertyChanged( () => Visible );
            }
        }

        private Guid Generator
        {
            get { return _model.Generator; }
        }

        private string _content = string.Empty;
        private bool _isSelected;
        private bool _visible;

        private readonly IPasswordEditorModel _parentModel;
        private readonly Settings _settings;
        private readonly IDerivedPasswordModel _model;
    }
}