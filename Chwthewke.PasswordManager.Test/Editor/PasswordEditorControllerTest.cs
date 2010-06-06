using System;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorControllerTest
    {
        [ SetUp ]
        public void SetUpController( )
        {
            _storeMock = new Mock<IPasswordRepository>( );
            _digester = new PasswordDigester( new Sha512Factory( ), new NullTimeProvider( ) );
            _controller = new PasswordEditorController( _storeMock.Object, _digester, ( ) => _guid,
                                                        PasswordGenerators.All );
        }

        [ Test ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsDirty, Is.False );
            Assert.That( _controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsPasswordLoaded, Is.False );
            Assert.That( _controller.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _controller.MasterPasswordId, Is.Null );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.Null );
            Assert.That( _controller.SelectedGenerator, Is.Null );
            Assert.That( _controller.Generators, Is.EquivalentTo( PasswordGenerators.All ) );
        }

        [ Test ]
        public void KeyModificationMakesDirty( )
        {
            // Setup
            // Exercise
            _controller.Key = "abcd";
            // Verify
            Assert.That( _controller.IsDirty, Is.True );
        }

        [ Test ]
        public void NoteModificationMakesDirty( )
        {
            // Setup
            // Exercise
            _controller.Note = "abcd";
            // Verify
            Assert.That( _controller.IsDirty, Is.True );
        }

        [ Test ]
        public void ChangeMasterPasswordMakesEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( _controller.IsDirty );
        }


        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            _controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutASignificantKey( )
        {
            // Setup

            // Exercise
            _controller.Key = "  \t";
            _controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAMasterPassword( )
        {
            // Setup

            // Exercise
            _controller.Key = "abcd";
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
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


        [ Test ]
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

        [ Test ]
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

        [ Test ]
        public void MasterPasswordIdIsAsFoundInStore( )
        {
            // Setup
            Guid guid = new Guid( "AEBE0ECF-D80D-48AE-B9BE-1EF4B2D72605" );
            SecureString masterPassword = "12456".ToSecureString( );
            _storeMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( guid );
            // Exercise
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( _controller.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [ Test ]
        public void MasterPasswordIdNullIfNotFoundInStore( )
        {
            // Setup
            SecureString masterPassword = "12456".ToSecureString( );
            _storeMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( ( Guid? ) null );
            // Exercise
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( _controller.MasterPasswordId, Is.Null );
        }

        [ Test ]
        public void SaveWithoutGeneratedPassword( )
        {
            // Setup
            _controller.Key = "abcd";
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( s => s.AddOrUpdate( It.IsAny<PasswordDigest>( ) ), Times.Never( ) );
        }

        [ Test ]
        public void SaveWithoutSelectedPassword( )
        {
            // Setup
            _controller.Key = "abcd";
            _controller.MasterPassword = "1234".ToSecureString( );
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( s => s.AddOrUpdate( It.IsAny<PasswordDigest>( ) ), Times.Never( ) );
        }

        [ Test ]
        public void SavePasswordMakesNotDirtyAndPasswordLoaded( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "1234".ToSecureString( );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.Full;

            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            _controller.Note = note;
            _controller.SelectedGenerator = generator;
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( _controller.IsDirty, Is.False );
            Assert.That( _controller.IsPasswordLoaded, Is.True );
        }

        [ Test ]
        public void SavePasswordWithKnownMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "1234".ToSecureString( );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.Full;

            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            _controller.Note = note;
            _controller.SelectedGenerator = generator;

            Guid guid = new Guid( "729486C6-05F9-46C3-AEF0-C745CB20DB8D" );
            _storeMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( guid );

            PasswordDigest expectedDigest = _digester.Digest( key,
                                                              generator.MakePassword( key, masterPassword ),
                                                              guid, generator.Id, note );
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( store => store.AddOrUpdate( expectedDigest ) );
        }


        [ Test ]
        public void SavePasswordWithUnknownMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "1234".ToSecureString( );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.AlphaNumeric;

            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            _controller.Note = note;
            _controller.SelectedGenerator = generator;

            _guid = new Guid( "729486C6-05F9-46C3-AEF0-C745CB20DB8D" );
            _storeMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( ( Guid? ) null );

            PasswordDigest expectedDigest = _digester.Digest( key,
                                                              generator.MakePassword( key, masterPassword ),
                                                              _guid, generator.Id, note );
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( store => store.AddOrUpdate( expectedDigest ) );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.Not.Null );
        }

        [ Test ]
        public void KeyIsStoredIffStoreHasKey( )
        {
            // Setup
            _storeMock.Setup( s => s.FindPasswordInfo( "abcd" ) ).Returns( new PasswordDigestBuilder( ).WithKey( "abcd" ) );
            // Exercise
            // Verify
            _controller.Key = "abcd";
            Assert.That( _controller.IsKeyStored, Is.True );
            _controller.Key = "abde";
            Assert.That( _controller.IsKeyStored, Is.False );
        }

        [ Test ]
        public void LoadPasswordSetsRelevantFields( )
        {
            // Setup
            Guid guid = new Guid( "EE5402AD-39FD-426D-B600-52A892BEF0E0" );
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" )
                .WithMasterPasswordId( guid );
            _storeMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            _controller.Key = digest.Key;
            _controller.LoadPassword( );
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( "abde" ) );
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( _controller.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( _controller.IsDirty, Is.False );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.EqualTo( guid ) );

            Assert.That( _controller.IsPasswordLoaded, Is.True );
        }

        [ Test ]
        public void LoadPasswordOnceOnly( )
        {
            // Setup
            Guid guid = new Guid( "EE5402AD-39FD-426D-B600-52A892BEF0E0" );
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" )
                .WithMasterPasswordId( guid );
            _storeMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            _controller.Key = digest.Key;
            _controller.LoadPassword( );
            _storeMock.Setup( store => store.FindPasswordInfo( digest.Key ) )
                .Returns( ( ) =>
                              {
                                  Assert.Fail( );
                                  return null;
                              } );
            // Exercise
            _controller.LoadPassword( );
            // Verify
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            _storeMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            _controller.Key = digest.Key;
            _controller.LoadPassword( );
            // Exercise
            _controller.Key = "abcd";
            // Verify
            Assert.That( _controller.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void DeleteHasNoEffectIfPasswordNotLoaded( )
        {
            // Setup
            string key = _controller.Key;
            // Exercise
            _controller.DeletePassword( );
            // Verify
            _storeMock.Verify( store => store.Remove( key ), Times.Never( ) );
        }

        private IPasswordEditorController _controller;
        private Mock<IPasswordRepository> _storeMock;
        private Guid _guid;
        private IPasswordDigester _digester;


        private class NullTimeProvider : ITimeProvider
        {
            public DateTime Now
            {
                get { return new DateTime( 0 ); }
            }
        }
    }
}