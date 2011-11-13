using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor2
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
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Verify
            Assert.That( App.ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( App.ViewModel.DerivedPasswords.First( s => s.Generator == PasswordGenerators.AlphaNumeric ).IsSelected );
            Assert.That( App.ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( App.ViewModel.Title, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordAllowsDeleteCommand( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );

            // Exercise
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Verify
            Assert.That( App.ViewModel.DeleteCommand.CanExecute( null ), Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordLoaded( )
        {
            // Setup

            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Verify
            Assert.That( App.ViewModel.IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Exercise
            App.ViewModel.Key = "abcd";
            // Verify
            Assert.That( App.ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            App.ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            App.ViewModel.UpdateMasterPassword( string.Empty.ToSecureString( ) );
            // Verify
            Assert.That( App.ViewModel.DerivedPasswords.Any( slot => slot.IsSelected ) );
        }

        [ Test ]
        public void DeletePasswordWhenCommandAvailable( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );
            // Exercise
            App.ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.Null );
        }

        [ Test ]
        public void DeletePasswordChangesCanExecuteDelete( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );

            bool deleteChanged = false;
            App.ViewModel.DeleteCommand.CanExecuteChanged += ( s, e ) => deleteChanged = true;
            // Exercise
            App.ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.Null );
            Assert.That( deleteChanged );
        }

        [ Test ]
        public void DeletePasswordRaisesStoreModified( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );

            bool storeModifiedRaised = false;
            App.ViewModel.StoreModified += ( s, e ) => { storeModifiedRaised = true; };
            // Exercise
            App.ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( storeModifiedRaised, Is.True );
        }

        [ Test ]
        public void DeletePasswordDoesNotResetEditor( )
        {
            // Setup
            AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abde" );

            App.ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            App.ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( App.ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( App.ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( App.ViewModel.DerivedPasswords.First( slot => slot.IsSelected ).Generator,
                         Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( App.ViewModel.IsKeyReadonly, Is.False );
        }
    }
}