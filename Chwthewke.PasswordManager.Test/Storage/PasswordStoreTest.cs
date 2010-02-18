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
            PasswordInfo stored = new PasswordInfo( "myKey", new byte[ ] { 0x55, 0xad }, new Guid( ), new DateTime( ),
                                                    "a Note" );
            // Exercise
            _passwordStorage.AddOrUpdate( stored );
            PasswordInfo retrieved = _passwordStorage.FindPasswordInfo( "myKey" );
            // Verify
            Assert.That( retrieved, Is.EqualTo( stored ) );
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordStorage.Passwords, Contains.Item( stored ) );
        }

        [ Test ]
        public void UpdateMakesNewPasswordInfoAvailable( )
        {
            // Setup
            PasswordInfo storedFirst = new PasswordInfo( "myKey", new byte[ ] { 0x55, 0xad }, new Guid( ),
                                                         new DateTime( ), "a Note" );
            _passwordStorage.AddOrUpdate( storedFirst );
            PasswordInfo updated = new PasswordInfo( "myKey", new byte[ ] { 0x84, 0xbb }, new Guid( ), new DateTime( ),
                                                     "a new Note" );
            // Exercise
            _passwordStorage.AddOrUpdate( updated );
            PasswordInfo retrieved = _passwordStorage.FindPasswordInfo( "myKey" );
            // Verify
            Assert.That( retrieved, Is.EqualTo( updated ) );
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordStorage.Passwords, Contains.Item( updated ) );
        }

        [ Test ]
        public void AddMultiplePasswordMakesAllAvailable( )
        {
            // Setup
            PasswordInfo storedFirst = new PasswordInfo( "myKey", new byte[ ] { 0x55, 0xad }, new Guid( ),
                                                         new DateTime( ), "a Note" );
            PasswordInfo storedSecond = new PasswordInfo( "myNewKey", new byte[ ] { 0x84, 0xbb }, new Guid( ),
                                                          new DateTime( ), "a new Note" );
            // Exercise
            _passwordStorage.AddOrUpdate( storedFirst );
            _passwordStorage.AddOrUpdate( storedSecond );
            // Verify
            Assert.That( _passwordStorage.Passwords, Has.Count.EqualTo( 2 ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "myKey" ), Is.EqualTo( storedFirst ) );
            Assert.That( _passwordStorage.FindPasswordInfo( "myNewKey" ), Is.EqualTo( storedSecond ) );
        }
    }
}