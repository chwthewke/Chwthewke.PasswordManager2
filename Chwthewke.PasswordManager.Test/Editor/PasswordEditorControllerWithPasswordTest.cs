using System;
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
    public class PasswordEditorControllerWithPasswordTest
    {
        [ SetUp ]
        public void SetUpController( )
        {
            _storeMock = new Mock<IPasswordStore>( );
            _digester = new PasswordDigester( new Sha512Factory( ), new NullTimeProvider( ) );
            _controller = new PasswordEditorController( _storeMock.Object, _digester, ( ) => _guid,
                                                        PasswordGenerators.All );

            _digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "a short note." );

            _storeMock.Setup( store => store.FindPasswordInfo( _digest.Key ) ).Returns( _digest );
            _controller.Key = _digest.Key;
            _controller.LoadPassword( );
        }


        [ Test ]
        public void ChangeNoteMakesEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.Note = string.Empty;
            // Verify
            Assert.That( _controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsDirty );
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
        public void ChangeSelectedGeneratorMakesEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.Full ) );
            Assert.That( _controller.IsDirty );
        }

        [ Test ]
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
            _storeMock.Verify( store => store.AddOrUpdate( It.Is<PasswordDigest>(
                d => d.Note == note && d.PasswordGeneratorId == generator.Id
                                                               ) ) );
        }


        [ Test ]
        public void SaveWhenNotDirtyHasNoEffect( )
        {
            // Setup
            // Exercise
            _controller.SavePassword( );
            // Verify
            _storeMock.Verify( store => store.AddOrUpdate( It.IsAny<PasswordDigest>( ) ), Times.Never( ) );
        }


        [ Test ]
        public void DeleteRemovesPasswordFromStore( )
        {
            // Setup
            string key = _controller.Key;
            // Exercise
            _controller.DeletePassword( );
            // Verify
            _storeMock.Verify( store => store.Remove( key ) );
        }

        [ Test ]
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
            _storeMock.Setup( s => s.FindPasswordInfo( key ) ).Returns( ( PasswordDigest ) null );

            Assert.That( _controller.Key, Is.EqualTo( key ) );
            Assert.That( _controller.Note, Is.EqualTo( note ) );
            Assert.That( _controller.MasterPassword, Is.EqualTo( masterPassword ) );
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( selectedGenerator ) );

            Assert.That( _controller.IsDirty, Is.True );
            Assert.That( _controller.IsPasswordLoaded, Is.False );
            Assert.That( _controller.IsKeyStored, Is.False );
        }


        private IPasswordEditorController _controller;
        private Mock<IPasswordStore> _storeMock;
        private readonly Guid _guid = new Guid( "{782F77BB-7482-4307-A246-E9A0BF2F5B86}" );
        private IPasswordDigester _digester;
        private PasswordDigest _digest;


        private class NullTimeProvider : ITimeProvider
        {
            public DateTime Now
            {
                get { return new DateTime( 0 ); }
            }
        }
    }
}