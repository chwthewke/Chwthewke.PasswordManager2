using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Tests.Storage
{
    [ TestFixture ]
    public class PasswordDictionaryTest : PasswordStorageTestBase
    {
        protected override IPasswordStorageService GetStorageService( )
        {
            return new PasswordDictionary( );
        }

        // HasPassword 

        [ Test ]
        public void TestHasPasswordReveals( )
        {
            // Setup
            const string expectedPassword = "generatedPassword";
            PasswordType expectedPasswordType = PasswordType.AlphaNumeric;
            // Exercise
            _passwordStorage.SetPassword( "myKey", expectedPassword, expectedPasswordType );
            // Verify
            Assert.That( _passwordStorage.HasPassword( "myKey" ), Is.True );
        }
    }
}