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
    [TestFixture]
    public class PasswordEditorControllerWithPasswordTest
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordDatabase PasswordDatabase { get; set; }
        
        public PasswordEditorControllerFactory ControllerFactory { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global

        private IPasswordEditorController _controller;

        private Mock<IMasterPasswordMatcher> _passwordMatcherMock;

        [SetUp]
        public void SetUpController( )
        {
            _passwordMatcherMock = new Mock<IMasterPasswordMatcher>( );

            AppSetUp.TestContainer(
                b =>
                    {
                        b.RegisterType<NullTimeProvider>( ).As<ITimeProvider>( );
                        b.RegisterInstance( _passwordMatcherMock.Object ).As<IMasterPasswordMatcher>( );
                        b.RegisterInstance( (Func<Guid>) ( ( ) => _guid ) ).As<Func<Guid>>( );
                    } ).InjectProperties( this );


            _digest = new PasswordDigestBuilder { Key = "abde", PasswordGeneratorId = PasswordGenerators.AlphaNumeric.Id, Note = "a short note." };

            PasswordDatabase.AddOrUpdate( _digest );

            _controller = ControllerFactory.PasswordEditorControllerFor( _digest.Key );

            //
            Assert.That( PasswordDatabase.FindByKey( "abde" ).Hash, Is.EqualTo( new byte[0] ) );
        }

        [Test]
        public void ChangeMasterPasswordDoesNotMakeEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( _controller.IsDirty, Is.False );
        }

        [Test]
        public void ChangeNoteMakesEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.Note = string.Empty;
            // Verify
            Assert.That( _controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _controller.IsDirty );
        }

        [Test]
        public void ChangeSelectedGeneratorMakesEditorDirty( )
        {
            // Setup
            // Exercise
            _controller.SelectedGenerator = PasswordGenerators.Full;
            // Verify
            Assert.That( _controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.Full ) );
            Assert.That( _controller.IsDirty );
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
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.FindByKey( "abde" ), Is.SameAs( _digest ) );
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

            Assert.That( _controller.IsDirty, Is.True );
            Assert.That( _controller.IsPasswordLoaded, Is.False );

            Assert.That( _controller.ExpectedMasterPasswordId, Is.Null );
        }


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