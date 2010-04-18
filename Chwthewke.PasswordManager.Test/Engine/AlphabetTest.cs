using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class AlphabetTest
    {
        [ Test ]
        public void TestConvertArrayToSymbols64( )
        {
            // Setup
            byte[ ] array = { 15, 22, 44, 60 };
            const string symbols64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-+";
            // Exercise
            string symbols = new Alphabet( symbols64 ).ToString( array );
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
            string symbols = new Alphabet( symbols10 ).ToString( array );
            // Verify
            Assert.That( symbols, Is.EqualTo( "1809785" ) );
        }

        [ Test ]
        public void AlphabetLengthIsSymbolsLength( )
        {
            // Setup
            const string symbols10 = "0123456789";
            // Exercise
            Alphabet alphabet = new Alphabet( symbols10 );
            // Verify
            Assert.That( alphabet.Length, Is.EqualTo( symbols10.Length ) );
        }
    }
}