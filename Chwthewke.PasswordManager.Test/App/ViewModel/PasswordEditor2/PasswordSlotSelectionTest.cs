using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor2
{
    [ TestFixture ]
    public class PasswordSlotSelectionTest : PasswordEditorTestBase
    {
        [ Test ]
        public void SelectingPasswordEnablesCopySave( )
        {
            // Setup
            App.ViewModel.Key = "abcd";
            App.ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );

            bool copyCommandCanExecuteChanged = false;
            App.ViewModel.CopyCommand.CanExecuteChanged += ( s, e ) => { copyCommandCanExecuteChanged = true; };
            bool saveCommandCanExecuteChanged = false;
            App.ViewModel.SaveCommand.CanExecuteChanged += ( s, e ) => { saveCommandCanExecuteChanged = true; };
            // Exercise
            App.ViewModel.DerivedPasswords[ 0 ].IsSelected = true;
            // Verify
            Assert.That( App.ViewModel.CopyCommand.CanExecute( null ), Is.True );
            Assert.That( App.ViewModel.SaveCommand.CanExecute( null ), Is.True );
            Assert.That( copyCommandCanExecuteChanged, Is.True );
            Assert.That( saveCommandCanExecuteChanged, Is.True );
        }
    }
}