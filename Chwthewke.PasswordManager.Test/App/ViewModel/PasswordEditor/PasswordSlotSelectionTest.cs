using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class PasswordSlotSelectionTest : PasswordEditorTestBase
    {
        [ Test ]
        public void SelectingPasswordEnablesCopySave( )
        {
            // Setup
            ViewModel.Key = "abcd";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );

            bool copyCommandCanExecuteChanged = false;
            ViewModel.CopyCommand.CanExecuteChanged += ( s, e ) => { copyCommandCanExecuteChanged = true; };
            bool saveCommandCanExecuteChanged = false;
            ViewModel.SaveCommand.CanExecuteChanged += ( s, e ) => { saveCommandCanExecuteChanged = true; };
            // Exercise
            ViewModel.Slots[ 0 ].IsSelected = true;
            // Verify
            Assert.That( ViewModel.CopyCommand.CanExecute( null ), Is.True );
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            Assert.That( copyCommandCanExecuteChanged, Is.True );
            Assert.That( saveCommandCanExecuteChanged, Is.True );
        }

    }
}