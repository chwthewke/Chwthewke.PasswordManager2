using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class SecureTest
    {
        [ Test ]
        public void TestSecureStringExtraction( )
        {
            // Setup
            const string s = "a String.";
            SecureString sec = Wrap( s );
            // Exercise
            byte[ ] bytes = Secure.GetBytes( Encoding.UTF8, sec );
            // Verify
            Assert.That( bytes, Is.EquivalentTo( Encoding.UTF8.GetBytes( s ) ) );
        }

        [ Test ]
        public void TestSecureStringExtractionWithSpecialChars( )
        {
            // Setup
            const string s = "a String with ŠƤĔċǐĀļ characters.";
            SecureString sec = Wrap( s );
            // Exercise
            byte[ ] bytes = Secure.GetBytes( Encoding.UTF8, sec );
            // Verify
            Assert.That( bytes, Is.EquivalentTo( Encoding.UTF8.GetBytes( s ) ) );
        }

        internal static SecureString Wrap( string s )
        {
            SecureString result = new SecureString( );
            for ( int i = 0 ; i < s.Length ; ++i )
                result.AppendChar( s[ i ] );
            return result;
        }
    }
}