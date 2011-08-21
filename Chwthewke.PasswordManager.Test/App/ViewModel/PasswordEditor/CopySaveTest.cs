using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Test.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class CopySaveTest : PasswordEditorTestBase
    {
        [ Test ]
        public void CopyPasswordWhenCommandAvailable( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            PasswordSlotViewModel slot = ViewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( ViewModel.CopyCommand.CanExecute( null ), Is.True );
            // Exercise
            ViewModel.CopyCommand.Execute( null );
            // Verify
            ClipboardServiceMock.Verify( cs => cs.CopyToClipboard( It.Is<string>( s => s == slot.Content ) ) );
        }

        [ Test ]
        public void SavePasswordWhenCommandAvailable( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            PasswordSlotViewModel slot = ViewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            // Exercise
            ViewModel.SaveCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abc" ), Is.Not.Null );
        }

        [ Test ]
        public void SavePasswordRaisesStoreModified( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            PasswordSlotViewModel slot = ViewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            bool storeModifiedRaised = false;
            ViewModel.StoreModified += ( s, e ) => { storeModifiedRaised = true; };
            // Exercise
            ViewModel.SaveCommand.Execute( null );
            // Verify
            Assert.That( storeModifiedRaised, Is.True );
        }

        [ Test ]
        public void KeyChangeRaisesCanExecuteChangedForSaveCopy( )
        {
            // Setup
            bool saveChanged = false;
            bool copyChanged = false;
            ViewModel.SaveCommand.CanExecuteChanged += ( s, e ) => saveChanged = true;
            ViewModel.CopyCommand.CanExecuteChanged += ( s, e ) => copyChanged = true;
            // Exercise
            ViewModel.Key = "abc";
            // Verify
            Assert.That( saveChanged );
            Assert.That( copyChanged );
        }

        [ Test ]
        public void MasterPasswordChangeRaisesCanExecuteChangedForSaveCopy( )
        {
            // Setup
            bool saveChanged = false;
            bool copyChanged = false;
            ViewModel.SaveCommand.CanExecuteChanged += ( s, e ) => saveChanged = true;
            ViewModel.CopyCommand.CanExecuteChanged += ( s, e ) => copyChanged = true;
            // Exercise
            ViewModel.UpdateMasterPassword( "abc".ToSecureString( ) );
            // Verify
            Assert.That( saveChanged );
            Assert.That( copyChanged );
        }
    }
}