using System.Linq;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordMaterializerTest
    {
        [ Test ]
        public void TestConvertArrayToSymbols64Returns12Characters( )
        {
            // Setup
            const string symbols64 = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789-+";
            PasswordMaterializer materializer = new PasswordMaterializer( symbols64 );

            BaseConverter converter = new BaseConverter( 64 );

            byte[ ] input = { 0xA1, 0x22, 0x44, 0x60, 0xE7, 0x01, 0x1A, 0x48, 0xCB };
            byte[ ] convertedBytes = converter.ConvertBytesToDigits( input, 12 );

            string expectedOutput = new string( convertedBytes.Select( b => symbols64[ b ] ).ToArray( ) );
            // Exercise
            string output = materializer.ToString( input );
            // Verify
            Assert.That( output, Is.EqualTo( expectedOutput ) );
        }

        [ Test ]
        public void TestConvertArrayToSymbols16Returns16Characters( )
        {
            // Setup
            const string symbols16 = "0123456789ABCDEF";
            PasswordMaterializer materializer = new PasswordMaterializer( symbols16 );

            byte[ ] input = { 0xA1, 0x22, 0x44, 0x60, 0xE7, 0x01, 0x1A, 0x48 };
            // Exercise
            string symbols = materializer.ToString( input );
            // Verify
            Assert.That( symbols, Is.EqualTo( "A1224460E7011A48" ) );
        }

        [ Test ]
        public void AlphabetNeeds8BytesWith16Symbols( )
        {
            // Set up
            const string symbols16 = "0123456789ABCDEF";
            PasswordMaterializer materializer = new PasswordMaterializer( symbols16 );

            // Exercise
            // Verify
            Assert.That( materializer.BytesNeeded, Is.EqualTo( 8 ) );
        }

        [Test]
        public void AlphabetNeeds9BytesWith128Symbols( )
        {
            // Set up
            string symbols128 = Enumerable.Repeat( "0123456789ABCDEF", 8 ).Aggregate( ( l, r ) => l + r );
            PasswordMaterializer materializer = new PasswordMaterializer( symbols128 );

            // Exercise
            // Verify
            Assert.That( materializer.BytesNeeded, Is.EqualTo( 8 ) );
        }

        [Test]
        public void AlphabetNeeds9BytesWith48Symbols( )
        {
            // Set up
            string symbols128 = Enumerable.Repeat( "0123456789ABCDEF", 3 ).Aggregate( ( l, r ) => l + r );
            PasswordMaterializer materializer = new PasswordMaterializer( symbols128 );

            // Exercise
            // Verify
            Assert.That( materializer.BytesNeeded, Is.EqualTo( 9 ) );
        }

     }
}