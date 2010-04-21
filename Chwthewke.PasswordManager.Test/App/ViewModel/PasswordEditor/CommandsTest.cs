using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class CommandsTest : TestWithStoreBase
    {

        [ Test ]
        public void CopyPasswordWhenCommandAvailable( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            PasswordSlotViewModel slot = ViewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( ViewModel.CopyCommand.CanExecute( null ), Is.True );
            // Exercise
            ViewModel.CopyCommand.Execute( null );
            // Verify
            ClipboardServiceMock.Verify( cs => cs.CopyToClipboard( It.Is<string>( s => s == slot.Content ) ) );
        }

        [ Test ]
        public void SavePasswordWhenAvailable( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            PasswordSlotViewModel slot = ViewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            // Exercise
            ViewModel.SaveCommand.Execute( null );
            // Verify
            StoreMock.Verify( store => store.AddOrUpdate( 
                It.Is<PasswordDigest>( d => d == Editor.GeneratedPassword( slot.Generator ).SavablePasswordDigest ) ) );
        }
    }
}