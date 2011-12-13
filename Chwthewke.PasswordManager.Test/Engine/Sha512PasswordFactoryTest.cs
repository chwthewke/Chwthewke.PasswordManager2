using System;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class Sha512PasswordFactoryTest
    {
        private IDerivedKeyFactory _derivedKeyFactory;

        [ SetUp ]
        public void SetupDerivedKeyFactory( )
        {
            _derivedKeyFactory = new Sha512DerivedKeyFactory( ( s, p ) => PasswordGenerators.InternalSalt.Concat( p ).Concat( s ).ToArray( ) );
        }

        /*
         * Useful unix stuff to get test values.
         * 
         * printf "\xAA\x01" | shasum -a 512
         * ... | cut -c1-128 | sed -e 's/\(..\)/\\x\1/g' -> result injectable into printf for iteration
         * ... | cut -c1-128 | sed -e 's/\(..\)/0x\1, /g' -> result copyable into a C# array
         */

        // ~$ printf "tsU&yUaZulAs4eOVpasswordsalt" | shasum -a 512
        // 72f7eaeeefdf2630005a1db93d6a0c10fcb86aa1b1fa073cc540f85a8d9a2b50cf77a6bfdcfa0959e30ef5667741e63e3bf1b1f16b6929d926671266b19ecdf0
        [ Test ]
        public void DeriveWithSingleIteration( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "salt" ), Encoding.UTF8.GetBytes( "password" ), 1, 64 );
            // Verify
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0x72, 0xf7, 0xea, 0xee, 0xef, 0xdf, 0x26, 0x30,
                                             0x00, 0x5a, 0x1d, 0xb9, 0x3d, 0x6a, 0x0c, 0x10,
                                             0xfc, 0xb8, 0x6a, 0xa1, 0xb1, 0xfa, 0x07, 0x3c,
                                             0xc5, 0x40, 0xf8, 0x5a, 0x8d, 0x9a, 0x2b, 0x50,
                                             0xcf, 0x77, 0xa6, 0xbf, 0xdc, 0xfa, 0x09, 0x59,
                                             0xe3, 0x0e, 0xf5, 0x66, 0x77, 0x41, 0xe6, 0x3e,
                                             0x3b, 0xf1, 0xb1, 0xf1, 0x6b, 0x69, 0x29, 0xd9,
                                             0x26, 0x67, 0x12, 0x66, 0xb1, 0x9e, 0xcd, 0xf0,
                                         } ) );
            Console.WriteLine( string.Join( ", ", derived.Select( b => b < 128 ? string.Format( "0x{0:x2}", b ) : string.Format( "-0x{0:x2}", 256 - b ) ) ) );
        }

        // ~$ printf "tsU&yUaZulAs4eOVpássw0rðsalt" | shasum -a 512
        // d80d4707f96d7d6979bf14c39d99606dc50eeec16d108d64e08812c12dc5131259fa5c3f391ee4b16a2c26a14993cd4d7b3b80429c3ed3cf738e078326521feb
        [ Test ]
        public void DeriveNonAsciiPasswordWithSingleIteration( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "salt" ), Encoding.UTF8.GetBytes( "pássw0rð" ), 1, 64 );
            // Verify
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0xd8, 0x0d, 0x47, 0x07, 0xf9, 0x6d, 0x7d, 0x69,
                                             0x79, 0xbf, 0x14, 0xc3, 0x9d, 0x99, 0x60, 0x6d,
                                             0xc5, 0x0e, 0xee, 0xc1, 0x6d, 0x10, 0x8d, 0x64,
                                             0xe0, 0x88, 0x12, 0xc1, 0x2d, 0xc5, 0x13, 0x12,
                                             0x59, 0xfa, 0x5c, 0x3f, 0x39, 0x1e, 0xe4, 0xb1,
                                             0x6a, 0x2c, 0x26, 0xa1, 0x49, 0x93, 0xcd, 0x4d,
                                             0x7b, 0x3b, 0x80, 0x42, 0x9c, 0x3e, 0xd3, 0xcf,
                                             0x73, 0x8e, 0x07, 0x83, 0x26, 0x52, 0x1f, 0xeb,
                                         } ) );
            Console.WriteLine( string.Join( ", ", derived.Select( b => b < 128 ? string.Format( "0x{0:x2}", b ) : string.Format( "-0x{0:x2}", 256 - b ) ) ) );
        }

        // ~$ printf "tsU&yUaZulAs4eOVpassword\x72\xf7\xea\xee\xef\xdf\x26\x30\x00\x5a\x1d\xb9\x3d\x6a\x0c\x10\xfc\xb8\x6a\xa1\xb1\xfa\x07\x3c\xc5\x40\xf8\x5a\x8d\x9a\x2b\x50\xcf\x77\xa6\xbf\xdc\xfa\x09\x59\xe3\x0e\xf5\x66\x77\x41\xe6\x3e\x3b\xf1\xb1\xf1\x6b\x69\x29\xd9\x26\x67\x12\x66\xb1\x9e\xcd\xf0" | shasum -a 512
        // 5425ea16e255a699dc611e8f18ca4e0071e773e4368115160035ed18bf2287755c2d43b28dbd8e6f9cb73107527605f5bd378fd440870eec18286645fce65553
        [ Test ]
        public void DeriveWithSecondIterationReusesResult( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "salt" ), Encoding.UTF8.GetBytes( "password" ), 2, 64 );
            // Verify
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0x54, 0x25, 0xea, 0x16, 0xe2, 0x55, 0xa6, 0x99,
                                             0xdc, 0x61, 0x1e, 0x8f, 0x18, 0xca, 0x4e, 0x00,
                                             0x71, 0xe7, 0x73, 0xe4, 0x36, 0x81, 0x15, 0x16,
                                             0x00, 0x35, 0xed, 0x18, 0xbf, 0x22, 0x87, 0x75,
                                             0x5c, 0x2d, 0x43, 0xb2, 0x8d, 0xbd, 0x8e, 0x6f,
                                             0x9c, 0xb7, 0x31, 0x07, 0x52, 0x76, 0x05, 0xf5,
                                             0xbd, 0x37, 0x8f, 0xd4, 0x40, 0x87, 0x0e, 0xec,
                                             0x18, 0x28, 0x66, 0x45, 0xfc, 0xe6, 0x55, 0x53,
                                         } ) );
            Console.WriteLine( string.Join( ", ", derived.Select( b => b < 128 ? string.Format( "0x{0:x2}", b ) : string.Format( "-0x{0:x2}", 256 - b ) ) ) );
        }
    }
}