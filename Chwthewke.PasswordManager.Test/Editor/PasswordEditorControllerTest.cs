using System;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorControllerTest
    {
        [ SetUp ]
        public void SetUpController( )
        {
            _storeMock = new Mock<IPasswordStore>( );
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
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            _controller.MasterPassword = Util.Secure( "12345" );
            // Verify
            Assert.That( _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutASignificantKey( )
        {
            // Setup

            // Exercise
            _controller.Key = "  \t";
            _controller.MasterPassword = Util.Secure( "12345" );
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
            SecureString masterPassword = Util.Secure( "12345" );
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
            SecureString masterPassword = Util.Secure( "12345" );
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
            SecureString masterPassword = Util.Secure( "12345" );
            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            // Exercise
            _controller.MasterPassword = Util.Secure( string.Empty );
            // Verify
            Assert.That(
                _controller.Generators.All( g => _controller.GeneratedPassword( g ) == string.Empty ),
                Is.True );
        }

        [ Test ]
        public void MasterPasswordIdIsAsFoundByStore( )
        {
            // Setup
            Guid guid = new Guid( "AEBE0ECF-D80D-48AE-B9BE-1EF4B2D72605" );
            SecureString masterPassword = Util.Secure( "12456" );
            _storeMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( guid );
            // Exercise
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( _controller.MasterPasswordId, Is.EqualTo( guid ) );
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
            _controller.MasterPassword = Util.Secure( "1234" );
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( s => s.AddOrUpdate( It.IsAny<PasswordDigest>( ) ), Times.Never( ) );
        }

        [ Test ]
        public void SavePasswordWithKnownMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = Util.Secure( "1234" );
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


        [Test]
        public void SavePasswordWithUnknownMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = Util.Secure( "1234" );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.AlphaNumeric;

            _controller.Key = key;
            _controller.MasterPassword = masterPassword;
            _controller.Note = note;
            _controller.SelectedGenerator = generator;

            _guid = new Guid( "729486C6-05F9-46C3-AEF0-C745CB20DB8D" );
            _storeMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( (Guid?)null );

            PasswordDigest expectedDigest = _digester.Digest( key,
                                                              generator.MakePassword( key, masterPassword ),
                                                              _guid, generator.Id, note );
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( store => store.AddOrUpdate( expectedDigest ) );
        }

        private IPasswordEditorController _controller;
        private Mock<IPasswordStore> _storeMock;
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