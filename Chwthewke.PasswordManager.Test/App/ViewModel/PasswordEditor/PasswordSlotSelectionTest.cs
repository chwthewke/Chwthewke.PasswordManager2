using System.Windows.Input;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class PasswordSlotSelectionTest : TestBase
    {
        [ Test ]
        public void SelectingPasswordEnablesCopySave( )
        {
            // Setup
            ViewModel.Key = "abcd";
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            bool requeryWasSuggested = false;
            CommandManager.RequerySuggested += ( s, e ) => { requeryWasSuggested = true; };
            // Exercise
            ViewModel.Slots[ 0 ].IsSelected = true;
            // Verify
            Assert.That( ViewModel.CopyCommand.CanExecute( null ), Is.True );
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            //Assert.That( requeryWasSuggested );
        }

        [ Test ]
        public void InvalidatingKeyDeselectsPassword( )
        {
            // Setup
            ViewModel.Key = "abcd";
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            ViewModel.Slots[ 0 ].IsSelected = true;
            bool requeryWasSuggested = false;
            CommandManager.RequerySuggested += ( s, e ) => { requeryWasSuggested = true; };
            // Exercise
            ViewModel.Key = string.Empty;
            // Verify
            Assert.That( ViewModel.Slots[ 0 ].IsSelected, Is.False );
            Assert.That( ViewModel.CopyCommand.CanExecute( null ), Is.False );
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.False );
            //Assert.That( requeryWasSuggested );
        }
    }
}