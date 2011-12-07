using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class SecureStringExtensionsTest
    {
        [ Test ]
        public void ConsumBytesAllowsAccesToSecurePassword( )
        {
            // Set up
            SecureString secureString = "abcd1234".ToSecureString( );
            // Exercise
            string extracted = secureString.ConsumeBytes( Encoding.UTF8, b => Encoding.UTF8.GetString( b ) );
            // Verify
            Assert.That( extracted, Is.EqualTo( "abcd1234" ) );
        }
    }
}