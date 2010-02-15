using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordEngineTest
    {
        [ Test ]
        public void TestStrong( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( Entropy( PasswordType.Ascii ), Is.AtLeast( 9 ) );
        }

        [ Test ]
        public void TestWeak( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( Entropy( PasswordType.AlphaNumeric ), Is.AtLeast( 9 ) );
        }

        private static int Entropy( PasswordType passwordType )
        {
            return new ArrayBaseConverter( passwordType._symbols.Length ).UsedBytes( passwordType._length );
        }
    }
}