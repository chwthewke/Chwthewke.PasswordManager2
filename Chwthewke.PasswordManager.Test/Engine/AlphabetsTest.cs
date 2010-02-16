using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [TestFixture]
    public class AlphabetsTest
    {
        private Encoding _ascii;

        [SetUp]
        public void SetUpAsciiEncoding( )
        {
            _ascii = Encoding.GetEncoding( Encoding.ASCII.CodePage, EncoderFallback.ExceptionFallback,
                                           DecoderFallback.ReplacementFallback );
        }

        [Test]
        public void TestSymbols92IsAscii7( )
        {
            // Setup
            // Exercise
            byte[ ] asciiBytes = _ascii.GetBytes( AllSymbols( Alphabets.Symbols92 ) );
            // Verify
            Assert.That( asciiBytes, Is.All.LessThan( 128 ) );
        }

        [Test]
        public void TestSymbols62IsAscii7( )
        {
            // Setup
            // Exercise
            byte[ ] asciiBytes = _ascii.GetBytes( AllSymbols( Alphabets.Symbols92 ) );
            // Verify
            Assert.That( asciiBytes, Is.All.LessThan( 128 ) );
        }

        private static string AllSymbols( Alphabet alphabet )
        {
            return alphabet.ToString( Enumerable.Range( 0, alphabet.Length - 1 ).Select( t => ( byte ) t ).ToArray( ) );
        }
    }
}