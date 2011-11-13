using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor2
{
    [ TestFixture ]
    public class ReloadingTest : PasswordEditorTestBase
    {
        [ Test ]
        public void ReloadingDatabaseUpdatesDirtiness( )
        {
            // Set up
            AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abc" );
            App.ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );

            Assert.That( App.ViewModel.SaveCommand.CanExecute( null ), Is.False );
            bool canSaveChanged = false;
            App.ViewModel.SaveCommand.CanExecuteChanged += ( s, e ) => canSaveChanged = true;


            AddPassword( "abc", "A note", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            App.ViewModel.UpdateFromDatabase( );
            // Verify
            Assert.That( App.ViewModel.SaveCommand.CanExecute( null ), Is.True );
            Assert.That( canSaveChanged, Is.True );
        }

        [ Test ]
        public void ReloadingDatabaseKeepsNoteAndSelectedGenerator( )
        {
            // Set up
            AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            App.ViewModel = ViewModelFactory.PasswordEditorFor( "abc" );
            App.ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );


            AddPassword( "abc", "A note", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            App.ViewModel.UpdateFromDatabase( );
            // Verify
            Assert.That( App.ViewModel.Note, Is.EqualTo( string.Empty ) );
            Assert.That( App.ViewModel.DerivedPasswords.First( s => s.Generator == PasswordGenerators.Full ).IsSelected, Is.True );
        }
    }
}