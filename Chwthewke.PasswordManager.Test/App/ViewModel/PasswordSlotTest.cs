using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordSlotTest
    {
        [ SetUp ]
        public void SetUp( )
        {
            IPasswordGenerator generator = new Mock<IPasswordGenerator>( ).Object;
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
        public void IsSelectedPropertyChanged( )
        {
            // Setup
            bool propertyChanged = false;
            _viewModel.PropertyChanged += ( s, e ) => { propertyChanged |= e.PropertyName == "IsSelected"; };
            // Exercise
            _viewModel.IsSelected = true;
            // Verify
            Assert.That( propertyChanged );
        }

        private PasswordSlotViewModel _viewModel;
    }
}