using Chwthewke.PasswordManager.App.ViewModel;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App
{
    [TestFixture]
    public class PasswordEditorViewModelTest
    {
        private PasswordEditorViewModel _viewModel;

        [SetUp]
        public void SetUpPasswordEditorViewModel( )
        {
            _viewModel = new PasswordEditorViewModel( );
        }

        [ Test, Ignore ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _viewModel.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _viewModel.Title, Is.EqualTo( "(new)" ) );
            // Assert.That commands.CanExecute( ) is false... 
        }

        [ Test ]
        public void TabHeaderUpdatedByKeyUpdates( )
        {
            // Setup
            // Exercise
            _viewModel.Key = "abc";
            // Verify
            Assert.That( _viewModel.Title, Is.EqualTo( "abc*" ) );
        }
    }
}