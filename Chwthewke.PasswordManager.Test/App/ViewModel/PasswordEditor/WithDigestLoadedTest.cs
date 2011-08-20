using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class WithDigestLoadedTest : PasswordEditorTestBase
    {
        [ Test ]
        public void LoadPasswordSetsRelevantFields( )
        {
            // Setup

            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );

            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Slots.First( s => s.Generator == PasswordGenerators.AlphaNumeric ).IsSelected );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.Title, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordAllowsDeleteCommand( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );

            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Verify
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordLoaded( )
        {
            // Setup

            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Verify
            Assert.That( ViewModel.IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateMasterPassword( string.Empty.ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.Slots.Any( slot => slot.IsSelected ) );
        }

        [Test]
        public void DeletePasswordWhenCommandAvailable( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.Null );
        }

        [Test]
        public void DeletePasswordChangesCanExecuteDelete( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );

            bool deleteChanged = false;
            ViewModel.DeleteCommand.CanExecuteChanged += ( s, e ) => deleteChanged = true;
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.Null );
            Assert.That( deleteChanged );
        }

        [Test]
        public void DeletePasswordRaisesStoreModified( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );

            bool storeModifiedRaised = false;
            ViewModel.StoreModified += ( s, e ) => { storeModifiedRaised = true; };
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( storeModifiedRaised, Is.True );
        }

        [ Test ]
        public void DeletePasswordDoesNotResetEditor( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );

            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.Slots.First( slot => slot.IsSelected ).Generator,
                         Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( ViewModel.IsKeyReadonly, Is.False );
        }
    }
}