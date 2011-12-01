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

            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );

            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.DerivedPasswords.First( s => s.Model.Generator == PasswordGenerators.LegacyAlphaNumeric ).IsSelected );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.Title, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordAllowsDeleteCommand( )
        {
            // Setup
            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );

            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            // Verify
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordLoaded( )
        {
            // Setup

            AddPassword( "abde", PasswordGenerators.LegacyFull, 1, "123".ToSecureString( ), string.Empty );
            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            // Verify
            Assert.That( ViewModel.IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            AddPassword( "abde", PasswordGenerators.LegacyFull, 1, "123".ToSecureString( ), string.Empty );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), string.Empty );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateMasterPassword( string.Empty.ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.DerivedPasswords.Any( slot => slot.IsSelected ) );
        }

        [ Test ]
        public void DeletePasswordWhenCommandAvailable( )
        {
            // Setup
            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordRepository.LoadPassword( "abde" ), Is.Null );
        }

        [ Test ]
        public void DeletePasswordChangesCanExecuteDelete( )
        {
            // Setup
            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            bool deleteChanged = false;
            ViewModel.DeleteCommand.CanExecuteChanged += ( s, e ) => deleteChanged = true;
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordRepository.LoadPassword( "abde" ), Is.Null );
            Assert.That( deleteChanged );
        }

        [ Test ]
        public void DeletePasswordRaisesStoreModified( )
        {
            // Setup
            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

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
            AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.DerivedPasswords.First( slot => slot.IsSelected ).Model.Generator,
                         Is.EqualTo( PasswordGenerators.LegacyAlphaNumeric ) );
            Assert.That( ViewModel.IsKeyReadonly, Is.False );
        }
    }
}