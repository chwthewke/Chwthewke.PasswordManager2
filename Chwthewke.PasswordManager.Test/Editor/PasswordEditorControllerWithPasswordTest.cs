using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorControllerWithPasswordTest : PasswordEditorControllerTestBase
    {
        [ SetUp ]
        public void SetUpController( )
        {
            StorePasswordAndGetMasterPasswordId( "abde", PasswordGenerators.AlphaNumeric, "1234".ToSecureString( ) );

            Controller = ControllerFactory.PasswordEditorControllerFor( "abde" );
        }

        [ Test ]
        public void ChangeMasterPasswordMakesEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( Controller.IsSaveable, Is.True );
        }

        [ Test ]
        public void UpdateMasterPasswordChangesItInDatabase( )
        {
            // Set up
            Guid previousMasterPasswordId = Controller.ExpectedMasterPasswordId.GetValueOrDefault( );
            Controller.MasterPassword = "123456".ToSecureString( );
            // Exercise
            Controller.SavePassword( );
            // Verify
            Guid newMasterPasswordId = PasswordDatabase.FindByKey( Controller.Key ).MasterPasswordId;
            Assert.That( newMasterPasswordId, Is.Not.EqualTo( previousMasterPasswordId ) );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.EqualTo( newMasterPasswordId ) );
            Assert.That( Controller.MasterPasswordId, Is.EqualTo( newMasterPasswordId ) );
        }

        [ Test ]
        public void ChangeNoteMakesEditorDirty( )
        {
            // Setup
            Controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            Controller.Note = "Now a note.";
            // Verify
            Assert.That( Controller.Note, Is.EqualTo( "Now a note." ) );
            Assert.That( Controller.IsSaveable, Is.True );
        }

        [ Test ]
        public void ChangeSelectedGeneratorMakesEditorDirty( )
        {
            // Setup
            Controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            Controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( Controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.Full ) );
            Assert.That( Controller.IsSaveable, Is.True );
        }

        [ Test ]
        public void ChangesWithoutAMasterPasswordDoNotDirtyEditor( )
        {
            // Set up

            // Exercise
            Controller.Note = "yadda";
            Controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( Controller.IsSaveable, Is.False );
        }

        [ Test ]
        public void SaveWithDifferentNoteAndGeneratorUpdatesStore( )
        {
            // Setup
            const string note = "a somewhat longer note.";
            IPasswordGenerator generator = PasswordGenerators.Full;
            Controller.Note = note;
            Controller.SelectedGenerator = generator;

            Controller.MasterPassword = "123456".ToSecureString( );
            // Exercise
            Controller.SavePassword( );
            // Verify

            PasswordDigest digest = PasswordDatabase.FindByKey( "abde" );
            Assert.That( digest.Hash, Is.Not.EqualTo( new byte[0] ) );
            Assert.That( digest.Note, Is.EqualTo( note ) );
            Assert.That( digest.PasswordGeneratorId, Is.EqualTo( generator.Id ) );
        }


        [ Test ]
        public void SaveWhenNotDirtyHasNoEffect( )
        {
            // Setup
            PasswordDigest digest = PasswordDatabase.FindByKey( "abde" );
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.SameAs( digest ) );
        }

        [ Test ]
        public void UpdatePasswordUpdatesModificationTimeButNotCreationTime( )
        {
            // Set up
            var now = new DateTime( 2011, 8, 16 );
            TimeProviderMock.Setup( p => p.Now ).Returns( now );
            // Exercise
            Controller.MasterPassword = "1234".ToSecureString( );
            Controller.Note = "updated note";
            Controller.SavePassword( );
            // Verify
            PasswordDigest newPasswordDigest = PasswordDatabase.FindByKey( Controller.Key );
            Assert.That( newPasswordDigest.ModificationTime, Is.EqualTo( now ) );
            Assert.That( newPasswordDigest.CreationTime, Is.EqualTo( Now ) );
            Assert.That( newPasswordDigest.ModificationTime, Is.Not.EqualTo( Now ) );
        }

        [ Test ]
        public void ReloadChangedPasswordMakesDirty( )
        {
            // Set up
            var now = new DateTime( 2011, 8, 16 );
            TimeProviderMock.Setup( p => p.Now ).Returns( now );
            StorePasswordAndGetMasterPasswordId( "abde", PasswordGenerators.Full, "4321".ToSecureString( ) );
            Controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            Controller.ReloadBaseline( );
            // Verify
            Assert.That( Controller.IsSaveable, Is.True );
        }

        [ Test ]
        public void ReloadChangedPasswordChangesExpectedMasterPassword( )
        {
            // Set up
            var now = new DateTime( 2011, 8, 16 );
            TimeProviderMock.Setup( p => p.Now ).Returns( now );
            Guid guid = StorePasswordAndGetMasterPasswordId( "abde", PasswordGenerators.Full, "4321".ToSecureString( ) );
            // Exercise
            Controller.ReloadBaseline( );
            // Verify
            Assert.That( Controller.ExpectedMasterPasswordId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void DeleteRemovesPasswordFromStore( )
        {
            // Setup
            // Exercise
            Controller.DeletePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.Null );
        }

        [ Test ]
        public void DeleteKeepsFieldsUntouched( )
        {
            // Setup
            string key = Controller.Key;
            string note = Controller.Note;
            SecureString masterPassword = Controller.MasterPassword;
            IPasswordGenerator selectedGenerator = Controller.SelectedGenerator;
            // Exercise
            Controller.DeletePassword( );
            // Verify

            Assert.That( Controller.Key, Is.EqualTo( key ) );
            Assert.That( Controller.Note, Is.EqualTo( note ) );
            Assert.That( Controller.MasterPassword, Is.EqualTo( masterPassword ) );
            Assert.That( Controller.SelectedGenerator, Is.EqualTo( selectedGenerator ) );

            Assert.That( Controller.IsSaveable, Is.False );
            Assert.That( Controller.IsPasswordLoaded, Is.False );

            Assert.That( Controller.ExpectedMasterPasswordId, Is.Null );
        }
    }
}