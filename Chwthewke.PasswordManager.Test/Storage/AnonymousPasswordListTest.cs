using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Tests.Storage
{
    [ TestFixture ]
    [ Ignore ]
    public class AnonymousPasswordListTest : PasswordStorageTestBase
    {
        protected override IPasswordStorageService GetStorageService( )
        {
            return new AnonymousPasswordList( );
        }

        [ Test ]
        public void TestHasPasswordDoesNotReveal( )
        {
            // Setup
            const string key = "myKey";
            const string expectedPassword = "generatedPassword";
            PasswordType expectedPasswordType = PasswordType.AlphaNumeric;
            // Exercise
            _passwordStorage.SetPassword( key, expectedPassword, expectedPasswordType );
            // Verify
            Assert.That( _passwordStorage.HasPassword( key ), Is.False );
        }
    }
}