using System;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordStoreTest
    {
        private IPasswordStore _passwordStorage;

        [ SetUp ]
        public void SetUpPasswordStore( )
        {
            _passwordStorage = new PasswordStore( );
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
            PasswordDigest stored = new PasswordDigest( "myKey", new byte[ ] { 0x55, 0xad }, default( Guid ),
                                                    default( Guid ), default( DateTime ), "a Note" );
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
            PasswordDigest storedFirst = new PasswordDigest( "myKey", new byte[ ] { 0x55, 0xad }, default( Guid ),
                                                         default( Guid ), default( DateTime ), "a Note" );
            _passwordStorage.AddOrUpdate( storedFirst );
            PasswordDigest updated = new PasswordDigest( "myKey", new byte[ ] { 0x84, 0xbb }, default( Guid ),
                                                     default( Guid ), default( DateTime ), "a new Note" );
            // Exercise
            _passwordStorage.AddOrUpdate( updated );
            PasswordDigest retrieved = _passwordStorage.FindPasswordInfo( "myKey" );
            // Verify
            Assert.That( retrieved, Is.EqualTo( updated ) );
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordStorage.Passwords, Contains.Item( updated ) );
        }

        [ Test ]
        public void AddMultiplePasswordMakesAllAvailable( )
        {
            // Setup
            PasswordDigest storedFirst = new PasswordDigest( "myKey", new byte[ ] { 0x55, 0xad }, default( Guid ),
                                                         default( Guid ), default( DateTime ), "a Note" );
            PasswordDigest storedSecond = new PasswordDigest( "myNewKey", new byte[ ] { 0x84, 0xbb }, default( Guid ),
                                                          default( Guid ), default( DateTime ), "a new Note" );
            // Exercise
            _passwordStorage.AddOrUpdate( storedFirst );
            _passwordStorage.AddOrUpdate( storedSecond );
            // Verify
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 2 ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "myKey" ), Is.EqualTo( storedFirst ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "myNewKey" ), Is.EqualTo( storedSecond ) );
        }

        [ Test ]
        public void RemovePasswordMakesItUnavailable( )
        {
            // Setup
            PasswordDigest stored = new PasswordDigest( "myKey", new byte[ ] { 0x55, 0xad }, default( Guid ),
                                                    default( Guid ), default( DateTime ), "a Note" );
            _passwordStorage.AddOrUpdate( stored );
            // Exercise
            _passwordStorage.Remove( stored );
            // Verify
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 0 ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "myKey" ), Is.Null );
        }

        [ Test ]
        public void RemovePasswordCopyMakesItUnavailable( )
        {
            // Setup
            PasswordDigest stored = new PasswordDigest( "myKey", new byte[ ] { 0x55, 0xad }, default( Guid ),
                                                    default( Guid ), default( DateTime ), "a Note" );
            PasswordDigest storedCopy = new PasswordDigest( "myKey", new byte[ ] { 0x55, 0xad }, stored.MasterPasswordId,
                                                        stored.PasswordSettingsId, stored.CreationTime, "a Note" );
            _passwordStorage.AddOrUpdate( stored );
            // Exercise
            _passwordStorage.Remove( storedCopy );
            // Verify
            Assert.That( storedCopy == stored );
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 0 ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "myKey" ), Is.Null );
        }
    }
}