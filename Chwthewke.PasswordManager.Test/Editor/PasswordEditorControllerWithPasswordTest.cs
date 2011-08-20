using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [TestFixture]
    public class PasswordEditorControllerWithPasswordTest : PasswordEditorControllerTestBase
    {

        [SetUp]
        public void SetUpController( )
        {
            StorePasswordAndGetMasterPasswordId( "abde", PasswordGenerators.AlphaNumeric, "1234".ToSecureString( ) );

            _controller = ControllerFactory.PasswordEditorControllerFor( "abde" );

        }

        [Test]
        public void ChangeMasterPasswordMakesEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( _controller.IsSaveable, Is.True );
        }

        [Test]
        public void UpdateMasterPasswordChangesItInDatabase( )
        {
            // Set up
            Guid previousMasterPasswordId = _controller.ExpectedMasterPasswordId.GetValueOrDefault( );
            _controller.MasterPassword = "123456".ToSecureString( );
            // Exercise
            _controller.SavePassword( );
            // Verify
            Guid newMasterPasswordId = PasswordDatabase.FindByKey( _controller.Key ).MasterPasswordId;
            Assert.That( newMasterPasswordId, Is.Not.EqualTo( previousMasterPasswordId ) );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.EqualTo( newMasterPasswordId ) );
            Assert.That( _controller.MasterPasswordId, Is.EqualTo( newMasterPasswordId ) );
        }

        [Test]
        public void ChangeNoteMakesEditorDirty( )
        {
            // Setup
            _controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            _controller.Note = "Now a note.";
            // Verify
            Assert.That( _controller.Note, Is.EqualTo( "Now a note." ) );
            Assert.That( _controller.IsSaveable, Is.True );
        }

        [Test]
        public void ChangeSelectedGeneratorMakesEditorDirty( )
        {
            // Setup
            _controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            _controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.Full ) );
            Assert.That( _controller.IsSaveable, Is.True );
        }

        [Test]
        public void ChangesWithoutAMasterPasswordDoNotDirtyEditor( )
        {
            // Set up
            
            // Exercise
            _controller.Note = "yadda";
            _controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( _controller.IsSaveable, Is.False );
        } 

        [Test]
        public void SaveWithDifferentNoteAndGeneratorUpdatesStore( )
        {
            // Setup
            const string note = "a somewhat longer note.";
            IPasswordGenerator generator = PasswordGenerators.Full;
            _controller.Note = note;
            _controller.SelectedGenerator = generator;

            _controller.MasterPassword = "123456".ToSecureString( );
            // Exercise
            _controller.SavePassword( );
            // Verify

            PasswordDigest digest = PasswordDatabase.FindByKey( "abde" );
            Assert.That( digest.Hash, Is.Not.EqualTo( new byte[0] ) );
            Assert.That( digest.Note, Is.EqualTo( note ) );
            Assert.That( digest.PasswordGeneratorId, Is.EqualTo( generator.Id ) );
        }


        [Test]
        public void SaveWhenNotDirtyHasNoEffect( )
        {
            // Setup
            PasswordDigest digest = PasswordDatabase.FindByKey( "abde" );
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.SameAs( digest ) );
        }

        [Test]
        public void UpdatePasswordUpdatesModificationTimeButNotCreationTime( )
        {
            // Set up
            var now = new DateTime( 2011, 8, 16 );
            _timeProviderMock.Setup( p => p.Now ).Returns( now );
            // Exercise
            _controller.MasterPassword = "1234".ToSecureString( );
            _controller.Note = "updated note";
            _controller.SavePassword( );
            // Verify
            PasswordDigest newPasswordDigest = PasswordDatabase.FindByKey( _controller.Key );
            Assert.That( newPasswordDigest.ModificationTime, Is.EqualTo( now ) );
            Assert.That( newPasswordDigest.CreationTime, Is.EqualTo( _now ) );
            Assert.That( newPasswordDigest.ModificationTime, Is.Not.EqualTo( _now ) );
        }

        [Test]
        public void DeleteRemovesPasswordFromStore( )
        {
            // Setup
            // Exercise
            _controller.DeletePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.Null );
        }

        [Test]
        public void DeleteKeepsFieldsUntouched( )
        {
            // Setup
            string key = _controller.Key;
            string note = _controller.Note;
            SecureString masterPassword = _controller.MasterPassword;
            IPasswordGenerator selectedGenerator = _controller.SelectedGenerator;
            // Exercise
            _controller.DeletePassword( );
            // Verify

            Assert.That( _controller.Key, Is.EqualTo( key ) );
            Assert.That( _controller.Note, Is.EqualTo( note ) );
            Assert.That( _controller.MasterPassword, Is.EqualTo( masterPassword ) );
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( selectedGenerator ) );

            Assert.That( _controller.IsSaveable, Is.False );
            Assert.That( _controller.IsPasswordLoaded, Is.False );

            Assert.That( _controller.ExpectedMasterPasswordId, Is.Null );
        }


    }
}