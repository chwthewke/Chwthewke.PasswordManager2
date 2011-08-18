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
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey(  );
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
            bool deleteChanged = false;
            ViewModel.DeleteCommand.CanExecuteChanged += ( s, e ) => deleteChanged = true;
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );

            // Exercise
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );
            // Verify
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.True );
            Assert.That( deleteChanged );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordLoaded( )
        {
            // Setup

            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );
            // Verify
            Assert.That( ViewModel.IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordKeepsPasswordSlotSelectionPossible( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abde";
            ViewModel.UpdateMasterPassword( "1234".ToSecureString( ) );
            ViewModel.LoadPasswordForKey( );
            // Verify
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
        }

        [ Test ]
        public void LoadPasswordPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );
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
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );
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
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );
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
            ViewModel.Key = "abde";
            ViewModel.LoadPasswordForKey( );

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
            ViewModel.Key = "abde";
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            ViewModel.LoadPasswordForKey( );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
            Assert.That( ViewModel.Slots.First( slot => slot.IsSelected ).Generator,
                         Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( ViewModel.IsKeyReadonly, Is.False );
        }
    }
}