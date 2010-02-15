using System;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    public abstract class BaseConverterTestBase {
        internal abstract IBaseConverter GetConverter( int theBase );

        [Test]
        public void Test12Base64DigitsNeeds72Bits( )
        {
            // Setup
            // Exercise
            IBaseConverter converter = GetConverter( 64 );
            // Verify
            Assert.That( converter.BytesNeeded( 12 ), Is.EqualTo( 9 ) );
        }

        [Test]
        public void Test8Base92DigitsNeeds53Bits( )
        {
            // Setup
            // Exercise
            IBaseConverter converter = GetConverter( 92 );
            // Verify
            Assert.That( converter.BytesNeeded( 8 ), Is.EqualTo( 7 ) );
        }

        [Test]
        public void TestCannotConvertToBaseOver256( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( new TestDelegate( ( ) => GetConverter( 257 ) ),
                         Throws.InstanceOf( typeof( ArgumentException ) ) );
        }

        [Test]
        public void TestCannotConvertWithNotEnoughBytes( )
        {
            // Setup
            IBaseConverter converter = GetConverter( 16 );
            // Exercise
            // Verify
            Assert.That( new TestDelegate( ( ) => converter.ConvertBytesToDigits( new byte[ ] { 0x00, 0x00 }, 5 ) ),
                         Throws.InstanceOf( typeof( ArgumentException ) ) );
        }

        [Test]
        public void TestConvertToBase64( )
        {
            // Setup
            byte[ ] src = { 0x11, 0xf7, 0xde };
            // lo-bits 1st, run-length bytes

            // 1      8 9     16 17    24
            // 10001000 11101111 01111011
            // becomes
            // 1    6 7   12 13  18 19  24
            // 100010 001110 111101 111011 
            byte[ ] expected = { 0x11, 0x1c, 0x2f, 0x37 };
            // Exercise
            byte[ ] actual = GetConverter( 64 ).ConvertBytesToDigits( src, 4 );
            // Verify
            Assert.That( actual, Is.EqualTo( expected ) );
        }

        [Test]
        public void TestConvertToBase100( )
        {
            // Setup
            byte[ ] src = { 0xfe, 0xff, 0xff, 0x00 }; // 16777214

            byte[ ] expected = { 14, 72, 77, 16 };
            // Exercise
            byte[ ] actual = GetConverter( 100 ).ConvertBytesToDigits( src, 4 );
            // Verify
            Assert.That( actual, Is.EqualTo( expected ) );
        }

        [Test]
        public void TestConvertWithExtraBytesInSource( )
        {
            // Setup
            byte[ ] src = { 0xff, 0xff, 0x01, 0x08 }; // 131071 + 8 * 2**24
            byte[ ] expected = { 71, 10, 13 }; // 3 bytes actually used -> 131071
            // Exercise
            byte[ ] actual = GetConverter( 100 ).ConvertBytesToDigits( src, 3 );
            // Verify
            Assert.That( actual, Is.EqualTo( expected ) );
        }
    }
}