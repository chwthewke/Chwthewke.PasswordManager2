using System;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [TestFixture]
    public class PasswordEditorControllerTest : PasswordEditorControllerTestBase
    {

        [SetUp]
        public void SetUpController( )
        {
            _controller = ControllerFactory.PasswordEditorControllerFor( string.Empty );
        }

        [Test]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsSaveable, Is.False );
            Assert.That( _controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsPasswordLoaded, Is.False );
            Assert.That( _controller.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _controller.MasterPasswordId, Is.Null );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.Null );
            Assert.That( _controller.SelectedGenerator, Is.Null );
            Assert.That( _controller.Generators, Is.EquivalentTo( PasswordGenerators.All ) );
        }

        [Test]
        public void KeyModificationOnlyDoesNotMakeDirty( )
        {
            // Setup
            // Exercise
            _controller.Key = "abcd";
            // Verify
            Assert.That( _controller.IsSaveable, Is.False );
        }

        [Test]
        public void NoteModificationOnlyDoesNotMakeDirty( )
        {
            // Setup
            // Exercise
            _controller.Note = "abcd";
            // Verify
            Assert.That( _controller.IsSaveable, Is.False );
        }

        [Test]
        public void ChangeMasterPasswordOnlyDoesNotMakeEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( _controller.IsSaveable, Is.False );
        }

        [Test]
        public void KeyAndMasterPasswordModificationPlusPasswordGeneratorSelectionMakeEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.Key = "abc";
            _controller.MasterPassword = "123456".ToSecureString( );
            _controller.SelectedGenerator = PasswordGenerators.AlphaNumeric;
            // Verify
            Assert.That( _controller.IsSaveable, Is.True );
        }


        [Test]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            _controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [Test]
        public void PasswordsAreNotGeneratedWithoutASignificantKey( )
        {
            // Setup

            // Exercise
            _controller.Key = "  \t";
            _controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [Test]
        public void PasswordsAreNotGeneratedWithoutAMasterPassword( )
        {
            // Setup

            // Exercise
            _controller.Key = "abcd";
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [Test]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "12345".ToSecureString( );
            // Exercise
            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That(
                _controller.Generators.All(
                    g => _controller.GeneratedPassword( g ) == g.MakePassword( key, masterPassword ) ),
                Is.True );
            Assert.That(
                _controller.Generators.All( g => _controller.GeneratedPassword( g ) != string.Empty ),
                Is.True );
        }


        [Test]
        public void PasswordsAreClearedAfterKeyClear( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "12345".ToSecureString( );
            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            // Exercise
            _controller.Key = string.Empty;
            // Verify
            Assert.That(
                _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ),
                Is.True );
        }

        [Test]
        public void PasswordsAreClearedAfterMasterPasswordClear( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "12345".ToSecureString( );
            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            // Exercise
            _controller.MasterPassword = string.Empty.ToSecureString( );
            // Verify
            Assert.That(
                _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ),
                Is.True );
        }

        [Test]
        public void MasterPasswordIdIsAsFoundInStore( )
        {
            // Setup

            SecureString masterPassword = "12456".ToSecureString( );
            Guid guid = StorePasswordAndGetMasterPasswordId( "abc", PasswordGenerators.AlphaNumeric, masterPassword );
            // Exercise
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( guid, Is.Not.EqualTo( default( Guid ) ) );
            Assert.That( _controller.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [Test]
        public void MasterPasswordIdNullIfNotFoundInStore( )
        {
            // Setup
            SecureString masterPassword = "12456".ToSecureString( );
            // Exercise
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( _controller.MasterPasswordId, Is.Null );
        }

        [Test]
        public void SaveWithoutGeneratedPassword( )
        {
            // Setup
            _controller.Key = "abcd";
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.Empty );
        }


        [Test]
        public void SaveWithoutSelectedPassword( )
        {
            // Setup
            _controller.Key = "abcd";
            _controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.Empty );
        }

        [Test]
        public void SavePasswordMakesNotDirtyAndPasswordLoaded( )
        {
            // Setup

            _controller.Key = "abcd";
            _controller.MasterPassword = "1234".ToSecureString( );
            _controller.Note = "some note";
            _controller.SelectedGenerator = PasswordGenerators.Full;
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( _controller.IsSaveable, Is.False );
            Assert.That( _controller.IsPasswordLoaded, Is.True );
        }

        [Test]
        public void SavePasswordWithKnownMasterPassword( )
        {
            // Setup
            SecureString masterPassword = "1234".ToSecureString( );
            Guid guid = StorePasswordAndGetMasterPasswordId( "abc", PasswordGenerators.AlphaNumeric, masterPassword );

            _controller.Key = "abcd";
            _controller.MasterPassword = masterPassword;
            _controller.Note = "some note";
            _controller.SelectedGenerator = PasswordGenerators.Full;


            PasswordDigest expectedDigest = Digester.Digest( "abcd",
                                                             PasswordGenerators.Full.MakePassword( "abcd", masterPassword ),
                                                             guid, PasswordGenerators.Full.Id, null, "some note" );
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abcd" ), Is.EqualTo( expectedDigest ) );
        }


        [Test]
        public void SavePasswordWithUnknownMasterPasswordSetsExpectedMasterPasswordToNewGuid( )
        {
            // Setup

            _controller.Key = "abcd";
            _controller.MasterPassword = "1234".ToSecureString( );
            _controller.Note = "some note";
            _controller.SelectedGenerator = PasswordGenerators.AlphaNumeric;

            // Exercise
            _controller.SavePassword( );
            // Verify
            PasswordDigest digest = PasswordDatabase.FindByKey( "abcd" );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.EqualTo( digest.MasterPasswordId ) );
        }

        [Test]
        public void LoadPasswordSetsRelevantFields( )
        {
            // Setup
            Guid guid = new Guid( "EE5402AD-39FD-426D-B600-52A892BEF0E0" );
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" )
                .WithMasterPasswordId( guid );
            PasswordDatabase.AddOrUpdate( digest );

            // Exercise

            _controller = ControllerFactory.PasswordEditorControllerFor( digest.Key );
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( "abde" ) );
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( _controller.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( _controller.IsSaveable, Is.False );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.EqualTo( guid ) );

            Assert.That( _controller.IsPasswordLoaded, Is.True );
        }

        [Test]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            PasswordDatabase.AddOrUpdate( digest );

            _controller = ControllerFactory.PasswordEditorControllerFor( digest.Key );
            // Exercise
            _controller.Key = "abcd";
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( "abde" ) );
        }

        [Test]
        public void DeleteHasNoEffectIfPasswordNotLoaded( )
        {
            // Setup
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abc" } );
            _controller.Key = "abcd";
            // Exercise
            _controller.DeletePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Has.Count.EqualTo( 1 ) );
        }

        [Test]
        public void NewPasswordIsSavedWithNowForCreationAndModification( )
        {
            // Set up
            _controller.Key = "abc";
            _controller.MasterPassword = "123".ToSecureString( );
            _controller.SelectedGenerator = PasswordGenerators.Full;
            // Exercise
            _controller.SavePassword( );
            // Verify
            PasswordDigest digest = PasswordDatabase.FindByKey( "abc" );
            Assert.That( digest.CreationTime, Is.EqualTo( _now ) );
            Assert.That( digest.ModificationTime, Is.EqualTo( _now ) );
        }

    }
}