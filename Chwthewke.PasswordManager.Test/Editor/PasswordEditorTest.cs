using System;
using System.Linq.Expressions;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorTest
    {
        [ SetUp ]
        public void SetUpEditor( )
        {
            _generator1Mock = CreateMockGenerator( "defaultGeneratedPassword1" );
            _generator2Mock = CreateMockGenerator( "defaultGeneratedPassword2" );

            _passwordDigesterMock = new Mock<IPasswordDigester>( );
            _digestAnything = d => d.Digest( It.IsAny<string>( ),
                                             It.IsAny<string>( ),
                                             It.IsAny<Guid>( ),
                                             It.IsAny<Guid>( ),
                                             It.IsAny<string>( ) );
            _passwordDigesterMock.Setup( _digestAnything ).Returns( new PasswordDigestBuilder( ) );

            _storageMock = new Mock<IPasswordStore>( );
            _storage = _storageMock.Object;

            _editor = new PasswordEditor( new[ ] { _generator1Mock.Object, _generator2Mock.Object },
                                          _passwordDigesterMock.Object,
                                          _storage );
        }

        private Mock<IPasswordGenerator> CreateMockGenerator( string defaultPassword )
        {
            var generator1Mock = new Mock<IPasswordGenerator>( );
            generator1Mock.Setup( g => g.MakePassword( It.IsAny<string>( ), It.IsAny<SecureString>( ) ) ).Returns(
                defaultPassword );
            return generator1Mock;
        }

        [ Test ]
        public void FreshEditorHasEmptyKeyNoGenPasswords( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _editor.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _editor.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _editor.SavedSlot, Is.Null );
            Assert.That( _editor.PasswordSlots.All( s => _editor.GeneratedPassword( s ) == null ) );
            Assert.That( _editor.PasswordSlots,
                         Is.EquivalentTo( new[ ] { _generator1Mock.Object, _generator2Mock.Object } ) );
        }

        [ Test ]
        public void GeneratePasswordFailsWithoutAKey( )
        {
            // Setup

            // Exercise
            Assert.That(
                new TestDelegate( ( ) => _editor.GeneratePasswords( Util.Secure( "mpmp" ) ) ),
                Throws.InstanceOf( typeof ( InvalidOperationException ) ) );
            // Verify
        }

        [ Test ]
        public void GeneratePasswordFailsWithAKeyMadeOfWhitespace( )
        {
            // Setup
            _editor.Key = "\t    \t ";
            // Exercise
            Assert.That(
                new TestDelegate( ( ) => _editor.GeneratePasswords( Util.Secure( "mpmp" ) ) ),
                Throws.InstanceOf( typeof ( InvalidOperationException ) ) );
            // Verify
        }

        [ Test ]
        public void GeneratePasswordUsesPasswordGenerators( )
        {
            // Setup
            _editor.Key = "aKey";
            SecureString masterPassword = Util.Secure( "mpmp" );
            _generator1Mock.Setup( pg => pg.MakePassword( "aKey", masterPassword ) ).Returns( "generatedPassword1" );
            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            _generator1Mock.Verify( pg => pg.MakePassword( "aKey", masterPassword ) );
            Assert.That( _editor.GeneratedPassword( _generator1Mock.Object ).GeneratedPassword,
                         Is.EqualTo( "generatedPassword1" ) );
            _generator2Mock.Verify( pg => pg.MakePassword( "aKey", masterPassword ) );
        }

        [ Test ]
        public void ModifyKeyResetsGeneratedPasswords( )
        {
            // Setup
            _editor.Key = "aKey";
            SecureString masterPassword = Util.Secure( "mpmp" );
            _generator1Mock.Setup( pg => pg.MakePassword( "aKey", masterPassword ) ).Returns( "generatedPassword1" );
            _editor.GeneratePasswords( masterPassword );
            // Exercise
            _editor.Key = "aNewKey";
            // Verify
            Assert.That( _editor.GeneratedPassword( _generator1Mock.Object ), Is.Null );
        }

        [ Test ]
        public void ResetReturnsToInitialState( )
        {
            // Setup
            _editor.Key = "aKey";
            SecureString masterPassword = Util.Secure( "mpmp" );
            _generator1Mock.Setup( pg => pg.MakePassword( "aKey", masterPassword ) ).Returns( "generatedPassword1" );
            _editor.GeneratePasswords( masterPassword );
            // Exercise
            _editor.Reset( );
            // Verify
            Assert.That( _editor.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _editor.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _editor.SavedSlot, Is.Null );
            Assert.That( _editor.PasswordSlots.All( s => _editor.GeneratedPassword( s ) == null ) );
            Assert.That( _editor.PasswordSlots,
                         Is.EquivalentTo( new[ ] { _generator1Mock.Object, _generator2Mock.Object } ) );
        }

        [ Test ]
        public void GeneratedPasswordDocumentsContainDigestsOfGeneratedPasswords( )
        {
            // Setup
            string key = "aKey";
            string generatedPassword = "generatedPassword1";
            string note = "a Note.";
            Guid generatorId = Guid.Parse( "DD6838A1-1091-447E-87DE-4022F9F9F246" );

            Expression<Func<IPasswordDigester, PasswordDigest>> digestPassword =
                d => d.Digest( It.Is<string>( k => k == key ),
                               It.Is<string>( p => p == generatedPassword ),
                               It.IsAny<Guid>( ),
                               It.Is<Guid>( g => g == generatorId ),
                               It.Is<string>( s => s == note ) );

            PasswordDigest passwordDigest = new PasswordDigestBuilder( );
            _passwordDigesterMock.Setup( digestPassword ).Returns( passwordDigest );


            _editor.Key = key;
            _editor.Note = note;
            SecureString masterPassword = Util.Secure( "mpmp" );

            _generator1Mock.Setup( g => g.MakePassword( key, masterPassword ) ).Returns( generatedPassword );
            _generator1Mock.Setup( g => g.Id ).Returns( generatorId );
            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            var passwordDocument = _editor.GeneratedPassword( _generator1Mock.Object );
            _passwordDigesterMock.Verify( digestPassword );
            Assert.That( passwordDocument.GeneratedPassword, Is.EqualTo( generatedPassword ) );
            Assert.That( passwordDocument.SavablePasswordDigest, Is.SameAs( passwordDigest ) );
        }

        [ Test ]
        public void GeneratedPasswordDocumentUsesMasterPasswordIdFromFinder( )
        {
            // Setup
            string key = "aKey";
            _editor.Key = key;
            SecureString masterPassword = Util.Secure( "mpmp" );

            Guid guid = Guid.Parse( "AC89E273-C063-4E2D-8A72-FE52B118A665" );
            _storageMock.Setup( f => f.IdentifyMasterPassword( masterPassword ) )
                .Returns( guid );

            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            _storageMock.Verify( f => f.IdentifyMasterPassword( masterPassword ) );
            _passwordDigesterMock.Verify( d => d.Digest( It.Is<string>( k => k == key ),
                                                         It.IsAny<string>( ),
                                                         It.Is<Guid>( g => g == guid ),
                                                         It.IsAny<Guid>( ),
                                                         It.IsAny<string>( ) ) );
        }

        [ Test ]
        public void GeneratedPasswordDocumentUsesNewNonZeroGuidIfFinderFails( )
        {
            // Setup
            string key = "aKey";
            _editor.Key = key;
            SecureString masterPassword = Util.Secure( "mpmp" );

            _storageMock.Setup( f => f.IdentifyMasterPassword( masterPassword ) )
                .Returns( ( Guid? ) null );

            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            _storageMock.Verify( f => f.IdentifyMasterPassword( masterPassword ) );
            _passwordDigesterMock.Verify( d => d.Digest( It.Is<string>( k => k == key ),
                                                         It.IsAny<string>( ),
                                                         It.Is<Guid>( g => g != default( Guid ) ),
                                                         It.IsAny<Guid>( ),
                                                         It.IsAny<string>( ) ) );
        }

        [ Test ]
        public void NoteChangeMutatesGeneratedDocuments( )
        {
            // Setup
            _editor.Key = "aKey";
            _editor.Note = "a Ntoe.";
            SecureString masterPassword = Util.Secure( "mpmp" );

            _editor.GeneratePasswords( masterPassword );
            string note = "a Note.";
            // Exercise
            _editor.Note = note;
            // Verify
            Assert.That(
                _editor.PasswordSlots.Select( s => _editor.GeneratedPassword( s ).SavablePasswordDigest.Note ),
                Is.All.EqualTo( note ) );
        }


        private IPasswordEditor _editor;

        private Mock<IPasswordGenerator> _generator1Mock;
        private Mock<IPasswordGenerator> _generator2Mock;
        private Mock<IPasswordDigester> _passwordDigesterMock;
        private IPasswordStore _storage;
        private Expression<Func<IPasswordDigester, PasswordDigest>> _digestAnything;
        private Mock<IPasswordStore> _storageMock;
    }
}