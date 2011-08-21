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
    [ TestFixture ]
    public class PasswordEditorControllerTest : PasswordEditorControllerTestBase
    {
        [ SetUp ]
        public void SetUpController( )
        {
            Controller = ControllerFactory.PasswordEditorControllerFor( string.Empty );
        }

        [ Test ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( Controller.Key, Is.EqualTo( string.Empty ) );
            Assert.That( Controller.IsSaveable, Is.False );
            Assert.That( Controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( Controller.IsPasswordLoaded, Is.False );
            Assert.That( Controller.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( Controller.MasterPasswordId, Is.Null );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.Null );
            Assert.That( Controller.SelectedGenerator, Is.Null );
            Assert.That( Controller.Generators, Is.EquivalentTo( PasswordGenerators.All ) );
        }

        [ Test ]
        public void KeyModificationOnlyDoesNotMakeDirty( )
        {
            // Setup
            // Exercise
            Controller.Key = "abcd";
            // Verify
            Assert.That( Controller.IsSaveable, Is.False );
        }

        [ Test ]
        public void NoteModificationOnlyDoesNotMakeDirty( )
        {
            // Setup
            // Exercise
            Controller.Note = "abcd";
            // Verify
            Assert.That( Controller.IsSaveable, Is.False );
        }

        [ Test ]
        public void ChangeMasterPasswordOnlyDoesNotMakeEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( Controller.IsSaveable, Is.False );
        }

        [ Test ]
        public void KeyAndMasterPasswordModificationPlusPasswordGeneratorSelectionMakeEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.Key = "abc";
            Controller.MasterPassword = "123456".ToSecureString( );
            Controller.SelectedGenerator = PasswordGenerators.AlphaNumeric;
            // Verify
            Assert.That( Controller.IsSaveable, Is.True );
        }


        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            Controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutASignificantKey( )
        {
            // Setup

            // Exercise
            Controller.Key = "  \t";
            Controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAMasterPassword( )
        {
            // Setup

            // Exercise
            Controller.Key = "abcd";
            // Verify
            Assert.That( Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "12345".ToSecureString( );
            // Exercise
            Controller.Key = key;
            Controller.MasterPassword = masterPassword;
            // Verify
            Assert.That(
                Controller.Generators.All(
                    g => Controller.GeneratedPassword( g ) == g.MakePassword( key, masterPassword ) ),
                Is.True );
            Assert.That(
                Controller.Generators.All( g => Controller.GeneratedPassword( g ) != string.Empty ),
                Is.True );
        }


        [ Test ]
        public void PasswordsAreClearedAfterKeyClear( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "12345".ToSecureString( );
            Controller.Key = key;
            Controller.MasterPassword = masterPassword;
            // Exercise
            Controller.Key = string.Empty;
            // Verify
            Assert.That(
                Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ),
                Is.True );
        }

        [ Test ]
        public void PasswordsAreClearedAfterMasterPasswordClear( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "12345".ToSecureString( );
            Controller.Key = key;
            Controller.MasterPassword = masterPassword;
            // Exercise
            Controller.MasterPassword = string.Empty.ToSecureString( );
            // Verify
            Assert.That(
                Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ),
                Is.True );
        }

        [ Test ]
        public void MasterPasswordIdIsAsFoundInStore( )
        {
            // Setup

            SecureString masterPassword = "12456".ToSecureString( );
            Guid guid = StorePasswordAndGetMasterPasswordId( "abc", PasswordGenerators.AlphaNumeric, masterPassword );
            // Exercise
            Controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( guid, Is.Not.EqualTo( default( Guid ) ) );
            Assert.That( Controller.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void MasterPasswordIdNullIfNotFoundInStore( )
        {
            // Setup
            SecureString masterPassword = "12456".ToSecureString( );
            // Exercise
            Controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( Controller.MasterPasswordId, Is.Null );
        }

        [ Test ]
        public void SaveWithoutGeneratedPassword( )
        {
            // Setup
            Controller.Key = "abcd";
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.Empty );
        }


        [ Test ]
        public void SaveWithoutSelectedPassword( )
        {
            // Setup
            Controller.Key = "abcd";
            Controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.Empty );
        }

        [ Test ]
        public void SavePasswordMakesNotDirtyAndPasswordLoaded( )
        {
            // Setup

            Controller.Key = "abcd";
            Controller.MasterPassword = "1234".ToSecureString( );
            Controller.Note = "some note";
            Controller.SelectedGenerator = PasswordGenerators.Full;
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( Controller.IsSaveable, Is.False );
            Assert.That( Controller.IsPasswordLoaded, Is.True );
        }

        [ Test ]
        public void SavePasswordWithKnownMasterPassword( )
        {
            // Setup
            SecureString masterPassword = "1234".ToSecureString( );
            Guid guid = StorePasswordAndGetMasterPasswordId( "abc", PasswordGenerators.AlphaNumeric, masterPassword );

            Controller.Key = "abcd";
            Controller.MasterPassword = masterPassword;
            Controller.Note = "some note";
            Controller.SelectedGenerator = PasswordGenerators.Full;


            PasswordDigest expectedDigest = Digester.Digest( "abcd",
                                                             PasswordGenerators.Full.MakePassword( "abcd", masterPassword ),
                                                             guid, PasswordGenerators.Full.Id, null, "some note" );
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abcd" ), Is.EqualTo( expectedDigest ) );
        }


        [ Test ]
        public void SavePasswordWithUnknownMasterPasswordSetsExpectedMasterPasswordToNewGuid( )
        {
            // Setup

            Controller.Key = "abcd";
            Controller.MasterPassword = "1234".ToSecureString( );
            Controller.Note = "some note";
            Controller.SelectedGenerator = PasswordGenerators.AlphaNumeric;

            // Exercise
            Controller.SavePassword( );
            // Verify
            PasswordDigest digest = PasswordDatabase.FindByKey( "abcd" );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.EqualTo( digest.MasterPasswordId ) );
        }

        [ Test ]
        public void LoadPasswordSetsRelevantFields( )
        {
            // Setup
            Guid guid = new Guid( "EE5402AD-39FD-426D-B600-52A892BEF0E0" );
            PasswordDigest digest = new PasswordDigestBuilder
                                        {
                                            Key = "abde",
                                            PasswordGeneratorId = PasswordGenerators.AlphaNumeric.Id,
                                            Note = "yadda yadda",
                                            MasterPasswordId = guid
                                        };
            PasswordDatabase.AddOrUpdate( digest );

            // Exercise

            Controller = ControllerFactory.PasswordEditorControllerFor( digest.Key );
            // Verify
            Assert.That( Controller.Key, Is.EqualTo( "abde" ) );
            Assert.That( Controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( Controller.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( Controller.IsSaveable, Is.False );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.EqualTo( guid ) );

            Assert.That( Controller.IsPasswordLoaded, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder { Key = "abde", PasswordGeneratorId = PasswordGenerators.Full.Id };
            PasswordDatabase.AddOrUpdate( digest );

            Controller = ControllerFactory.PasswordEditorControllerFor( digest.Key );
            // Exercise
            Controller.Key = "abcd";
            // Verify
            Assert.That( Controller.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void DeleteHasNoEffectIfPasswordNotLoaded( )
        {
            // Setup
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abc" } );
            Controller.Key = "abcd";
            // Exercise
            Controller.DeletePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Has.Count.EqualTo( 1 ) );
        }

        [ Test ]
        public void NewPasswordIsSavedWithNowForCreationAndModification( )
        {
            // Set up
            Controller.Key = "abc";
            Controller.MasterPassword = "123".ToSecureString( );
            Controller.SelectedGenerator = PasswordGenerators.Full;
            // Exercise
            Controller.SavePassword( );
            // Verify
            PasswordDigest digest = PasswordDatabase.FindByKey( "abc" );
            Assert.That( digest.CreationTime, Is.EqualTo( Now ) );
            Assert.That( digest.ModificationTime, Is.EqualTo( Now ) );
        }
    }
}