using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordSlotTest
    {
        [ SetUp ]
        public void SetUp( )
        {
            IPasswordGenerator generator = PasswordGenerators.Full;
            _viewModel = new PasswordSlotViewModel( generator );
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
    }
}