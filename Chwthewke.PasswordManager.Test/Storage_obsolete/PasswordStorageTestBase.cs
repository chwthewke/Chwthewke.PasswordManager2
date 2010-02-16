using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Tests.Storage
{
    public abstract class PasswordStorageTestBase
    {
        protected IPasswordStorageService _passwordStorage;
        protected abstract IPasswordStorageService GetStorageService( );

        [ SetUp ]
        public void SetUpRepository( )
        {
            _passwordStorage = GetStorageService( );
        }

        // HasPassword (common)

        [ Test ]
        public void TestValidateUnsavedPassword( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _passwordStorage.HasPassword( "anyKey" ), Is.False );
        }

        // ValidatePassword+SetPassword (common)

        [ Test ]
        public void TestValidatePasswordValidatesSamePasswordAndType( )
        {
            // Setup
            const string password = "generatedPassword";
            PasswordType passwordType = PasswordType.AlphaNumeric;
            // Exercise
            _passwordStorage.SetPassword( "myKey", password, passwordType );
            // Verify
            Assert.That( _passwordStorage.ValidatePassword( "myKey", password, passwordType ), Is.True );
        }


        [ Test ]
        public void TestValidatePasswordDoesNotValidateDifferentPassword( )
        {
            // Setup
            PasswordType passwordType = PasswordType.AlphaNumeric;
            // Exercise
            _passwordStorage.SetPassword( "myKey", "generatedPassword", passwordType );
            // Verify
            Assert.That( _passwordStorage.ValidatePassword( "myKey", "differentPassword", passwordType ), Is.False );
        }

        [ Test ]
        public void TestValidatePasswordDoesNotValidateDifferentPasswordType( )
        {
            // Setup
            const string password = "generatedPassword";
            // Exercise
            _passwordStorage.SetPassword( "myKey", password, PasswordType.AlphaNumeric );
            // Verify
            Assert.That( _passwordStorage.ValidatePassword( "myKey", "differentPassword", PasswordType.Ascii ),
                         Is.False );
        }


        [ Test ]
        public void TestValidatePasswordDoesNotValidateDifferentKey( )
        {
            // Setup
            // Exercise
            const string password = "generatedPassword";
            PasswordType passwordType = PasswordType.AlphaNumeric;
            _passwordStorage.SetPassword( "key1", password, passwordType );
            // Verify
            Assert.That( _passwordStorage.ValidatePassword( "key2", password, passwordType ), Is.False );
        }

        // ClearPassword (common)

        [ Test ]
        public void TestClearPassword( )
        {
            // Setup
            const string key = "key1";
            _passwordStorage.SetPassword( key, "pass1", PasswordType.AlphaNumeric );
            // Exercise
            _passwordStorage.ClearPassword( key );
            // Verify
            Assert.That( _passwordStorage.HasPassword( key ), Is.False );
        }
    }
}