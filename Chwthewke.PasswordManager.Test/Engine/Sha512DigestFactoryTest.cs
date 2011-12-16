using System;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class Sha512DigestFactoryTest
    {
        private IDerivedKeyFactory _derivedKeyFactory;

        [ SetUp ]
        public void SetupDerivedKeyFactory( )
        {
            _derivedKeyFactory = new Sha512DerivedKeyFactory( ( s, p ) => s.Concat( p ).ToArray( ) );
        }

        /*
         * Useful unix stuff to get test values.
         * 
         * printf "\xAA\x01" | shasum -a 512
         * ... | cut -c1-128 | sed -e 's/\(..\)/\\x\1/g' -> result injectable into printf for iteration
         * ... | cut -c1-128 | sed -e 's/\(..\)/0x\1, /g' -> result copyable into a C# array
         */

        [ Test ]
        public void DeriveDigest( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( PasswordGenerator.DigestSalt, Encoding.UTF8.GetBytes( "password" ), 1, 64 );
            // Verify
            Console.WriteLine( string.Join( ", ", derived.Select( b => string.Format( "0x{0:x2}", b ) ) ) );
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0x8b, 0x0e, 0x37, 0x48, 0x61, 0xb5, 0xbc, 0xe4,
                                             0x79, 0xf3, 0x8d, 0x70, 0x81, 0xf5, 0xea, 0x43,
                                             0xcc, 0xfc, 0xa4, 0x82, 0x36, 0x01, 0x59, 0xdc,
                                             0xd5, 0xfe, 0x22, 0x63, 0x49, 0xff, 0x92, 0xa2,
                                             0x62, 0xa6, 0x9e, 0xc9, 0xac, 0x8a, 0x30, 0x3f,
                                             0x0b, 0xdd, 0xf6, 0xd7, 0xf1, 0xac, 0xd6, 0x2f,
                                             0x8a, 0x16, 0x0e, 0x11, 0xd3, 0x49, 0xbf, 0x5e,
                                             0xc3, 0x8b, 0x23, 0xf8, 0x25, 0x54, 0x77, 0xc3
                                         } ) );
        }

        [ Test ]
        public void DeriveDigestWithNonAsciiPassword( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( PasswordGenerator.DigestSalt, Encoding.UTF8.GetBytes( "pássw0rð" ), 1, 64 );
            // Verify
            Console.WriteLine( string.Join( ", ", derived.Select( b => string.Format( "0x{0:x2}", b ) ) ) );
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0x01, 0x7b, 0x8c, 0xc2, 0xb5, 0xcb, 0xc9, 0x34,
                                             0x09, 0xe1, 0x64, 0x6e, 0xaa, 0x3e, 0x97, 0x93,
                                             0x25, 0x45, 0xdc, 0xbe, 0x6b, 0x99, 0xf0, 0x53,
                                             0xd5, 0xf6, 0x9b, 0x31, 0xdd, 0x6f, 0xa9, 0xa3,
                                             0xa6, 0xd2, 0x5e, 0xa1, 0xcf, 0x6f, 0xf2, 0x45,
                                             0xef, 0xfc, 0x56, 0x18, 0x79, 0xaf, 0x29, 0xb3,
                                             0x00, 0xc8, 0x1f, 0xa2, 0x6e, 0x65, 0xe8, 0xa7,
                                             0xe4, 0x6e, 0xd4, 0x5b, 0x92, 0xbf, 0xc4, 0xf3
                                         } ) );
        }
    }
}