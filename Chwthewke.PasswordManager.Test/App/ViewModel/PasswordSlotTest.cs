using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;
using Autofac;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    [ Ignore ]
    public class PasswordSlotTest
    {
        [ SetUp ]
        public void SetUp( )
        {
            IPasswordGenerator generator = PasswordGenerators.Full;
            _controller = AppSetUp.TestContainer( ).Resolve<IPasswordEditorController>( );
            _viewModel = new PasswordSlotViewModel( _controller, generator );
        }

        [ Test ]
        public void ContentPropertyChanged( )
        {
            // Setup
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "Content"; };
            // Exercise
            _viewModel.Content = "abc";
            // Verify
            Assert.That( propertyChanged );
        }

        [ Test ]
        public void IsSelectedPropertyChangedToTrue( )
        {
            // Setup
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };
            // Exercise
            _viewModel.IsSelected = true;
            // Verify
            Assert.That( propertyChanged );
        }

        [ Test ]
        public void IsSelectedPropertyChangedToFalse( )
        {
            // Setup
            _viewModel.IsSelected = true;
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };
            // Exercise
            _viewModel.IsSelected = false;
            // Verify
            Assert.That( propertyChanged );
        }

        private PasswordSlotViewModel _viewModel;
        private IPasswordEditorController _controller;
    }
}