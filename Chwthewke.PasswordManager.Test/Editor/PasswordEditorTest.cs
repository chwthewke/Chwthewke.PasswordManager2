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
            _generator1Mock = new Mock<IPasswordGenerator>( );
            _generator2Mock = new Mock<IPasswordGenerator>( );

            _masterPasswordFinderMock = new Mock<IMasterPasswordFinder>( );
            _passwordDigesterMock = new Mock<IPasswordDigester>( );

            _editor = new PasswordEditor( new[ ] { _generator1Mock.Object, _generator2Mock.Object },
                                          _masterPasswordFinderMock.Object, _passwordDigesterMock.Object );
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
            Assert.That( new TestDelegate( ( ) => _editor.GeneratePasswords( SecureTest.Wrap( "mpmp" ) ) ),
                         Throws.InstanceOf( typeof ( InvalidOperationException ) ) );
            // Verify
        }

        [ Test ]
        public void GeneratePasswordFailsWithAKeyMadeOfWhitespace( )
        {
            // Setup
            _editor.Key = "\t    \t ";
            // Exercise
            Assert.That( new TestDelegate( ( ) => _editor.GeneratePasswords( SecureTest.Wrap( "mpmp" ) ) ),
                         Throws.InstanceOf( typeof ( InvalidOperationException ) ) );
            // Verify
        }

        [ Test ]
        public void GeneratePasswordUsesPasswordGenerators( )
        {
            // Setup
            _editor.Key = "aKey";
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );
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
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );
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
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );
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
            Guid generatorId = Guid.Parse( "DD6838A1-1091-447E-87DE-4022F9F9F246" );

            Expression<Func<IPasswordDigester, PasswordDigest>> digestPassword =
                d => d.Digest( It.Is<string>( k => k == key ),
                               It.Is<string>( p => p == generatedPassword ),
                               It.IsAny<Guid>( ),
                               It.Is<Guid>( g => g == generatorId ),
                               It.Is<string>( s => s == string.Empty ) );

            PasswordDigest passwordDigest = new PasswordDigestBuilder( );
            _passwordDigesterMock.Setup( digestPassword ).Returns( passwordDigest );


            _editor.Key = key;
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );

            _generator1Mock.Setup( g => g.MakePassword( key, masterPassword ) ).Returns( generatedPassword );
            _generator1Mock.Setup( g => g.Id ).Returns( generatorId );
            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            var passwordDocument = _editor.GeneratedPassword( _generator1Mock.Object );
            _passwordDigesterMock.Verify( digestPassword );
            Assert.That( passwordDocument.GeneratedPassword, Is.EqualTo( generatedPassword ) );
            Assert.That( passwordDocument.SavablePasswordDigest, Is.SameAs( passwordDigest ) );
            //_masterPasswordFinderMock.Verify( f => f.IdentifyMasterPassword( masterPassword ) );
        }

        [ Test ]
        public void GeneratedPasswordDocumentUsesMasterPasswordIdFromFinder( )
        {
            // Setup
            string key = "aKey";
            _editor.Key = key;
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );

            Guid guid = Guid.Parse( "AC89E273-C063-4E2D-8A72-FE52B118A665" );
            _masterPasswordFinderMock.Setup( f => f.IdentifyMasterPassword( masterPassword ) )
                .Returns( guid );

            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            _masterPasswordFinderMock.Verify( f => f.IdentifyMasterPassword( masterPassword ) );
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
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );

            _masterPasswordFinderMock.Setup( f => f.IdentifyMasterPassword( masterPassword ) )
                .Returns( ( Guid? ) null );

            // Exercise
            _editor.GeneratePasswords( masterPassword );
            // Verify
            _masterPasswordFinderMock.Verify( f => f.IdentifyMasterPassword( masterPassword ) );
            _passwordDigesterMock.Verify( d => d.Digest( It.Is<string>( k => k == key ),
                                                         It.IsAny<string>( ),
                                                         It.Is<Guid>( g => g != default( Guid ) ),
                                                         It.IsAny<Guid>( ),
                                                         It.IsAny<string>( ) ) );
        }

        private IPasswordEditor _editor;

        private Mock<IPasswordGenerator> _generator1Mock;
        private Mock<IPasswordGenerator> _generator2Mock;
        private Mock<IMasterPasswordFinder> _masterPasswordFinderMock;
        private Mock<IPasswordDigester> _passwordDigesterMock;

        // Use cases, bitches !
        /*
         * IPasswordEditor _editor;
         * 
         * 1) Create new password "form"
         * 
         * _editor.Reset( );
         * Assert.That( _editor.Key == string.Empty );
         * Assert.That( _editor.SavedSlot == null );
         * Assert.That( _editor.GeneratedPassword( slot ) == null ); for any in PasswordSlots
         * 
         * 2) Saving a password
         * (after 1 above)
         * 
         * document.Key = smth
         * editor.GeneratePasswords( document, myMasterPassword )
         * document.GeneratedPasswords : list of Pairs <string, passwordDigest> (password, savable info)
         * 
         * remark : password info points to password settings & master password by hteir respective guid
         * 
         * 
         */
    }
}