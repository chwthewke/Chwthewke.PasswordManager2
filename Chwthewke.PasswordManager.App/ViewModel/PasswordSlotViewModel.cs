using System;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordSlotViewModel : ObservableObject
    {
        public PasswordSlotViewModel( IPasswordEditorController controller, IPasswordGenerator generator )
        {
            if ( generator == null )
                throw new ArgumentNullException( "generator" );
            _controller = controller;
            _generator = generator;

            _increaseIterationCommand = new RelayCommand( () => Iteration += 1 );
            _decreaseIterationCommand = new RelayCommand( () => Iteration -= 1, () => Iteration > 0 );
        }

        public IPasswordGenerator Generator
        {
            get { return _generator; }
        }

        public string GeneratorDescription
        {
            get { return Resources.ResourceManager.GetString( PasswordGeneratorTranslator.DescriptionKey( Generator ) ); }
        }

        public string Name
        {
            get { return Resources.ResourceManager.GetString( PasswordGeneratorTranslator.NameKey( Generator ) ); }
        }

        public int Iteration
        {
            get { return _iteration; }
            set
            {
                if ( value == _iteration )
                    return;
                _iteration = value;
                RaisePropertyChanged( () => Iteration );
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

        public ICommand IncreaseIterationCommand
        {
            get { return _increaseIterationCommand; }
        }

        public ICommand DecreaseIterationCommand
        {
            get { return _decreaseIterationCommand; }
        }

        public void Update( )
        {
            Content = _controller.GeneratedPassword( Generator );
            IsSelected = _controller.SelectedGenerator == Generator;
        }

        private string _content = string.Empty;
        private bool _isSelected;

        private readonly IPasswordEditorController _controller;
        private readonly IPasswordGenerator _generator;
        private int _iteration;

        private ICommand _increaseIterationCommand;
        private ICommand _decreaseIterationCommand;

    }
}