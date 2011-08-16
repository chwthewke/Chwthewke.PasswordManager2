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
    public class PasswordEditorControllerTest
    {
// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordDatabase PasswordDatabase { get; set; }

        public IPasswordDigester Digester { get; set; }

        public IPasswordEditorController Controller { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global


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
        }

        [Test]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( Controller.Key, Is.EqualTo( string.Empty ) );
            Assert.That( Controller.IsDirty, Is.False );
            Assert.That( Controller.Note, Is.EqualTo( string.Empty ) );
            Assert.That( Controller.IsPasswordLoaded, Is.False );
            Assert.That( Controller.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( Controller.MasterPasswordId, Is.Null );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.Null );
            Assert.That( Controller.SelectedGenerator, Is.Null );
            Assert.That( Controller.Generators, Is.EquivalentTo( PasswordGenerators.All ) );
        }

        [Test]
        public void KeyModificationMakesDirty( )
        {
            // Setup
            // Exercise
            Controller.Key = "abcd";
            // Verify
            Assert.That( Controller.IsDirty, Is.True );
        }

        [Test]
        public void NoteModificationMakesDirty( )
        {
            // Setup
            // Exercise
            Controller.Note = "abcd";
            // Verify
            Assert.That( Controller.IsDirty, Is.True );
        }

        [Test]
        public void ChangeMasterPasswordMakesEditorDirty( )
        {
            // Setup
            // Exercise
            Controller.MasterPassword = "123456".ToSecureString( );
            // Verify
            Assert.That( Controller.IsDirty );
        }


        [Test]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            Controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [Test]
        public void PasswordsAreNotGeneratedWithoutASignificantKey( )
        {
            // Setup

            // Exercise
            Controller.Key = "  \t";
            Controller.MasterPassword = "12345".ToSecureString( );
            // Verify
            Assert.That( Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [Test]
        public void PasswordsAreNotGeneratedWithoutAMasterPassword( )
        {
            // Setup

            // Exercise
            Controller.Key = "abcd";
            // Verify
            Assert.That( Controller.Generators.All( g => Controller.GeneratedPassword( g ) == string.Empty ), Is.True );
        }

        [Test]
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


        [Test]
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

        [Test]
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

        [Test]
        public void MasterPasswordIdIsAsFoundInStore( )
        {
            // Setup
            Guid guid = new Guid( "AEBE0ECF-D80D-48AE-B9BE-1EF4B2D72605" );
            SecureString masterPassword = "12456".ToSecureString( );
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( guid );
            // Exercise
            Controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( Controller.MasterPasswordId, Is.EqualTo( guid ) );
        }

        [Test]
        public void MasterPasswordIdNullIfNotFoundInStore( )
        {
            // Setup
            SecureString masterPassword = "12456".ToSecureString( );
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) )
                .Returns( (Guid?) null );
            // Exercise
            Controller.MasterPassword = masterPassword;
            // Verify
            Assert.That( Controller.MasterPasswordId, Is.Null );
        }

        [Test]
        public void SaveWithoutGeneratedPassword( )
        {
            // Setup
            Controller.Key = "abcd";
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.Empty );
        }


        [Test]
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

        [Test]
        public void SavePasswordMakesNotDirtyAndPasswordLoaded( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "1234".ToSecureString( );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.Full;

            Controller.Key = key;
            Controller.MasterPassword = masterPassword;
            Controller.Note = note;
            Controller.SelectedGenerator = generator;
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( Controller.IsDirty, Is.False );
            Assert.That( Controller.IsPasswordLoaded, Is.True );
        }

        [Test]
        public void SavePasswordWithKnownMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "1234".ToSecureString( );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.Full;

            Controller.Key = key;
            Controller.MasterPassword = masterPassword;
            Controller.Note = note;
            Controller.SelectedGenerator = generator;

            Guid guid = new Guid( "729486C6-05F9-46C3-AEF0-C745CB20DB8D" );
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( guid );

            PasswordDigest expectedDigest = Digester.Digest( key,
                                                             generator.MakePassword( key, masterPassword ),
                                                             guid, generator.Id, note );
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.EquivalentTo( new List<PasswordDigest> { expectedDigest } ) );
        }


        [Test]
        public void SavePasswordWithUnknownMasterPassword( )
        {
            // Setup
            string key = "abcd";
            SecureString masterPassword = "1234".ToSecureString( );
            string note = "some note";
            IPasswordGenerator generator = PasswordGenerators.AlphaNumeric;

            Controller.Key = key;
            Controller.MasterPassword = masterPassword;
            Controller.Note = note;
            Controller.SelectedGenerator = generator;

            _guid = new Guid( "729486C6-05F9-46C3-AEF0-C745CB20DB8D" );
            _passwordMatcherMock.Setup( s => s.IdentifyMasterPassword( masterPassword ) ).Returns( (Guid?) null );

            PasswordDigest expectedDigest = Digester.Digest( key,
                                                             generator.MakePassword( key, masterPassword ),
                                                             _guid, generator.Id, note );
            // Exercise
            Controller.SavePassword( );
            // Verify
            Assert.That( PasswordDatabase.Passwords, Is.EquivalentTo( new List<PasswordDigest> { expectedDigest } ) );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.Not.Null );
        }

        [Test]
        public void KeyIsStoredIffStoreHasKey( )
        {
            // Setup
            PasswordDatabase.AddOrUpdate( new PasswordDigestBuilder { Key = "abcd" } );
            // Exercise
            // Verify
            Controller.Key = "abcd";
            Assert.That( Controller.IsKeyStored, Is.True );
            Controller.Key = "abde";
            Assert.That( Controller.IsKeyStored, Is.False );
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
            Controller.Key = digest.Key;
            Controller.LoadPassword( );
            // Verify
            Assert.That( Controller.Key, Is.EqualTo( "abde" ) );
            Assert.That( Controller.SelectedGenerator, Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( Controller.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( Controller.IsDirty, Is.False );
            Assert.That( Controller.ExpectedMasterPasswordId, Is.EqualTo( guid ) );

            Assert.That( Controller.IsPasswordLoaded, Is.True );
        }

        [Test]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            PasswordDatabase.AddOrUpdate( digest );

            Controller.Key = digest.Key;
            Controller.LoadPassword( );
            // Exercise
            Controller.Key = "abcd";
            // Verify
            Assert.That( Controller.Key, Is.EqualTo( "abde" ) );
        }

        [Test]
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