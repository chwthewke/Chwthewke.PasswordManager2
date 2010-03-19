using System.Collections.Generic;
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

        [ Test ]
        public void TestSecureStringExtractionWithNonUnicodeEncoding( )
        {
            // Setup
            const string s = "é";
            SecureString sec = Wrap( s );
            // Exercise
            byte[ ] bytes = Secure.GetBytes( Encoding.GetEncoding( 1252 ), sec );
            // Verify
            Assert.That( bytes, Is.EquivalentTo( Encoding.GetEncoding( 1252 ).GetBytes( s ) ) );
            Assert.That( bytes, Is.Not.EquivalentTo( Encoding.UTF8.GetBytes( s ) ) );
        }

        internal static SecureString Wrap( IEnumerable<char> s )
        {
            SecureString result = new SecureString( );
            foreach ( char c in s )
                result.AppendChar( c );
            return result;
        }
    }
}