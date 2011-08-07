using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordStoreTest
    {
        private IPasswordRepository _passwordStorage;

        [ SetUp ]
        public void SetUpPasswordStore( )
        {
            _passwordStorage = new PasswordRepository( );
        }

        [ Test ]
        public void FindPasswordInfoReturnsNullForMissingInfo( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _passwordStorage.FindPasswordInfo( "anyKey" ), Is.Null );
        }

        [ Test ]
        public void AddMakesPasswordInfoAvailableToFindAndPasswords( )
        {
            // Setup
            PasswordDigest stored = new PasswordDigestBuilder( ).WithKey( "myKey" );
            // Exercise
            _passwordStorage.AddOrUpdate( stored );
            PasswordDigest retrieved = _passwordStorage.FindPasswordInfo( "myKey" );
            // Verify
            Assert.That( retrieved, Is.EqualTo( stored ) );
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordStorage.Passwords, Contains.Item( stored ) );
        }

        [ Test ]
        public void UpdateMakesNewPasswordInfoAvailable( )
        {
            // Setup
            PasswordDigest storedFirst =
                new PasswordDigestBuilder( ).WithKey( "aKey" ).WithHash( new byte[ ] { 0x55, 0xad } ).WithNote( "a Note" );
            _passwordStorage.AddOrUpdate( storedFirst );
            PasswordDigest updated =
                new PasswordDigestBuilder( ).WithKey( "aKey" ).WithHash( new byte[ ] { 0x84, 0xbb } ).WithNote(
                    "a new Note" );
            // Exercise
            _passwordStorage.AddOrUpdate( updated );
            PasswordDigest retrieved = _passwordStorage.FindPasswordInfo( "aKey" );
            // Verify
            Assert.That( retrieved, Is.EqualTo( updated ) );
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordStorage.Passwords, Contains.Item( updated ) );
        }

        [ Test ]
        public void AddMultiplePasswordMakesAllAvailable( )
        {
            // Setup
            PasswordDigest storedFirst = new PasswordDigestBuilder( ).WithKey( "aKey" );
            PasswordDigest storedSecond = new PasswordDigestBuilder( ).WithKey( "anotherKey" );
            // Exercise
            _passwordStorage.AddOrUpdate( storedFirst );
            _passwordStorage.AddOrUpdate( storedSecond );
            // Verify
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 2 ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "aKey" ), Is.EqualTo( storedFirst ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "anotherKey" ), Is.EqualTo( storedSecond ) );
        }

        [ Test ]
        public void RemovePasswordMakesItUnavailable( )
        {
            // Setup
            PasswordDigest stored = new PasswordDigestBuilder( ).WithKey( "aKey" );
            _passwordStorage.AddOrUpdate( stored );
            // Exercise
            _passwordStorage.Remove( "aKey" );
            // Verify
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 0 ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "aKey" ), Is.Null );
        }
    }
}