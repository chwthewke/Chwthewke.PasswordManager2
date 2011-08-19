using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;
using Autofac;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [TestFixture]
    [Ignore( "Failures" )]
    public class PasswordEditorControllerTest
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordDatabase PasswordDatabase { get; set; }

        public IPasswordDigester Digester { get; set; }

        public PasswordEditorControllerFactory ControllerFactory { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        private IPasswordEditorController _controller;


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
                        b.RegisterType<InMemoryPasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );
                    } )
                .InjectProperties( this );

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
            Guid guid = new Guid( "AEBE0ECF-D80D-48AE-B9BE-1EF4B2D72605" );
            SecureString masterPassword = "12456".ToSecureString( );
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( guid );
            // Exercise
            _controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( _controller.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [Test]
        public void MasterPasswordIdNullIfNotFoundInStore( )
        {
            // Setup
            SecureString masterPassword = "12456".ToSecureString( );
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( (Guid?) null );
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
            Assert.That( _controller.IsSaveable, Is.False );
            Assert.That( _controller.IsPasswordLoaded, Is.True );
        }

        [Test]
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
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( guid );

            PasswordDigest expectedDigest = Digester.Digest( key,
                                                             generator.MakePassword( key, masterPassword ),
                                                             guid, generator.Id, null, note );
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.EquivalentTo( new List<PasswordDigest> {expectedDigest} ) );
        }


        [Test]
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
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( (Guid?) null );

            PasswordDigest expectedDigest = Digester.Digest( key,
                                                             generator.MakePassword( key, masterPassword ),
                                                             _guid, generator.Id, null, note );
            // Exercise
            _controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.EquivalentTo( new List<PasswordDigest> {expectedDigest} ) );
            Assert.That( _controller.ExpectedMasterPasswordId, Is.Not.Null );
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
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder {Key = "abc"} );
            _controller.Key = "abcd";
            // Exercise
            _controller.DeletePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Has.Count.EqualTo( 1 ) );
        }

        private Guid _guid;
        private Mock<IMasterPasswordMatcher> _passwordMatcherMock;


        private class NullTimeProvider : ITimeProvider
        {
            public DateTime Now
            {
                get { return new DateTime( 0 ); }
            }
        }
    }
}