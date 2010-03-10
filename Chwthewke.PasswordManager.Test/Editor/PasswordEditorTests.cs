using System;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Test.Engine;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorTests
    {
        [ SetUp ]
        public void SetUpEditor( )
        {
            _generator1Mock = new Mock<IPasswordGenerator>( );
            _generator2Mock = new Mock<IPasswordGenerator>( );

            _editor = new PasswordEditor( new[ ] { _generator1Mock.Object, _generator2Mock.Object } );
        }

        [ Test ]
        public void FreshEditorHasEmptyKeyNoGenPasswords( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _editor.Key, Is.EqualTo( string.Empty ) );
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
            Assert.That( _editor.GeneratedPassword( _generator1Mock.Object ).GeneratedPassword, Is.EqualTo( "generatedPassword1" ) );
            _generator2Mock.Verify( pg => pg.MakePassword( "aKey", masterPassword ) );
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
            Assert.That( _editor.SavedSlot, Is.Null );
            Assert.That( _editor.PasswordSlots.All( s => _editor.GeneratedPassword( s ) == null ) );
            Assert.That( _editor.PasswordSlots,
                         Is.EquivalentTo( new[ ] { _generator1Mock.Object, _generator2Mock.Object } ) );
        }

        private IPasswordEditor _editor;

        private Mock<IPasswordGenerator> _generator1Mock;
        private Mock<IPasswordGenerator> _generator2Mock;

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