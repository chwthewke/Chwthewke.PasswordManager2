using System;
using System.Security;
using Autofac;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    [Ignore( "Failures..." )]
    public class PasswordEditorControllerWithPasswordTest
    {

        public IPasswordEditorController Controller { get; set; }
        public IPasswordDigester Digester { get; set; }
        public IPasswordDatabase Database { get; set; }

        private Mock<IMasterPasswordMatcher> _passwordMatcherMock;

        [ SetUp ]
        public void SetUpController( )
        {
            // TODO probably not very inspired to mock a repository.

            _passwordMatcherMock = new Mock<IMasterPasswordMatcher>( );

            AppSetUp.TestContainer(
                b =>
                {
                    b.RegisterType<NullTimeProvider>( ).As<ITimeProvider>( );
                    b.RegisterInstance( _passwordMatcherMock.Object ).As<IMasterPasswordMatcher>( );
                    b.RegisterInstance( ( Func<Guid> ) ( ( ) => _guid ) ).As<Func<Guid>>( );
                } ).InjectProperties( this );


            _digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "a short note." );

            Database.AddOrUpdate( _digest );

            Controller.Key = _digest.Key;
            Controller.LoadPassword( );
        }

        [Test]
        public void ChangeMasterPasswordDoesNotMakeEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( Controller.IsDirty, Is.False );
        }

        [ Test ]
        public void ChangeNoteMakesEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.Note = string.Empty;
            // Verify
            Assert.That( Controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( Controller.IsDirty );
        }

        [ Test ]
        public void ChangeSelectedGeneratorMakesEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( Controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.Full ) );
            Assert.That( Controller.IsDirty );
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
            _storeMock.Verify( store => store.AddOrUpdate( It.Is<PasswordDigest>(
                d => d.Note == note && d.PasswordGeneratorId == generator.Id
                                                               ) ) );
        }


        [ Test ]
        public void SaveWhenNotDirtyHasNoEffect( )
        {
            // Setup
            // Exercise
            Controller.SavePassword( );
            // Verify
            _storeMock.Verify( store => store.AddOrUpdate( It.IsAny<PasswordDigest>( ) ), Times.Never( ) );
        }


        [ Test ]
        public void DeleteRemovesPasswordFromStore( )
        {
            // Setup
            string key = Controller.Key;
            // Exercise
            Controller.DeletePassword( );
            // Verify
            _storeMock.Verify( store => store.Remove( key ) );
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
            _storeMock.Setup( s => s.FindPasswordInfo( key ) ).Returns( ( PasswordDigest ) null );

            Assert.That( Controller.Key, Is.EqualTo( key ) );
            Assert.That( Controller.Note, Is.EqualTo( note ) );
            Assert.That( Controller.MasterPassword, Is.EqualTo( masterPassword ) );
            Assert.That( Controller.SelectedGenerator, Is.EqualTo( selectedGenerator ) );

            Assert.That( Controller.IsDirty, Is.True );
            Assert.That( Controller.IsPasswordLoaded, Is.False );
            Assert.That( Controller.IsKeyStored, Is.False );

            Assert.That( Controller.ExpectedMasterPasswordId, Is.Null );
        }


        private Mock<IPasswordRepository> _storeMock;
        private readonly Guid _guid = new Guid( "{782F77BB-7482-4307-A246-E9A0BF2F5B86}" );
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