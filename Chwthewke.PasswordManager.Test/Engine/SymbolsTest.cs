using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class SymbolsTest
    {
        [ Test ]
        public void TestConvertArrayToSymbols64( )
        {
            // Setup
            byte[ ] array = { 15, 22, 44, 60 };
            const string symbols64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-+";
            // Exercise
            string symbols = new Symbols( symbols64 ).ToString( array );
            // Verify
            Assert.That( symbols, Is.EqualTo( "PWs8" ) );
        }

        [ Test ]
        public void TestConvertArrayToSymbols10( )
        {
            // Setup
            byte[ ] array = { 1, 8, 0, 9, 7, 8, 5 };
            const string symbols10 = "0123456789";
            // Exercise
            string symbols = new Symbols( symbols10 ).ToString( array );
            // Verify
            Assert.That( symbols, Is.EqualTo( "1809785" ) );
        }

        [ Test ]
        public void TestSymbolsLength92( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( Symbols.Symbols92, Has.Length.EqualTo( 92 ) );
        }

        [ Test ]
        public void TestSymbols92IsAscii7( )
        {
            // Setup
            // Exercise
            Encoding encoding = Encoding.GetEncoding( Encoding.ASCII.CodePage, EncoderFallback.ExceptionFallback,
                                                      DecoderFallback.ReplacementFallback );
            byte[ ] asciiBytes = encoding.GetBytes( Symbols.SymbolsString92 );
            // Verify
            Assert.That( asciiBytes, Is.All.LessThan( 128 ) );
        }

        [Test]
        public void TestSymbols62IsAscii7( )
        {
            // Setup
            // Exercise
            Encoding encoding = Encoding.GetEncoding( Encoding.ASCII.CodePage, EncoderFallback.ExceptionFallback,
                                                      DecoderFallback.ReplacementFallback );
            byte[ ] asciiBytes = encoding.GetBytes( Symbols.SymbolsString62 );
            // Verify
            Assert.That( asciiBytes, Is.All.LessThan( 128 ) );
        }


        [ Test ]
        public void TestSymbolsLength62( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( Symbols.Symbols62, Has.Length.EqualTo( 62 ) );
        }
    }
}