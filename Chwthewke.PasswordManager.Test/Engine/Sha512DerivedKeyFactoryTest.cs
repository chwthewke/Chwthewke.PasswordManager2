using System;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class Sha512DerivedKeyFactoryTest
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

        // ~$ printf "tsU&yUaZulAs4eOV1234abcd" | shasum -a 512
        // e132a0c4a6e249482b2cb74c9696addeec27237689b77209efa56a0f7be38e397c692a819d11524efcc15b983d6ed16d53e27278e1ace5e1656ec90ec91b8917
        [ Test ]
        public void DeriveWithSingleIteration( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "abcd" ), Encoding.UTF8.GetBytes( "1234" ), 1, 64 );
            // Verify
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0xe1, 0x32, 0xa0, 0xc4, 0xa6, 0xe2, 0x49, 0x48, 0x2b, 0x2c, 0xb7, 0x4c, 0x96, 0x96, 0xad, 0xde,
                                             0xec, 0x27, 0x23, 0x76, 0x89, 0xb7, 0x72, 0x09, 0xef, 0xa5, 0x6a, 0x0f, 0x7b, 0xe3, 0x8e, 0x39,
                                             0x7c, 0x69, 0x2a, 0x81, 0x9d, 0x11, 0x52, 0x4e, 0xfc, 0xc1, 0x5b, 0x98, 0x3d, 0x6e, 0xd1, 0x6d,
                                             0x53, 0xe2, 0x72, 0x78, 0xe1, 0xac, 0xe5, 0xe1, 0x65, 0x6e, 0xc9, 0x0e, 0xc9, 0x1b, 0x89, 0x17,
                                         } ) );
        }

        // ~$ printf "tsU&yUaZulAs4eOV1234abcd" | shasum -a 512
        // e132a0c4a6e249482b2cb74c9696addeec27237689b77209efa56a0f7be38e397c692a819d11524efcc15b983d6ed16d53e27278e1ace5e1656ec90ec91b8917
        [ Test ]
        public void DeriveNonAsciiPasswordWithSingleIteration( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "ábcð" ), Encoding.UTF8.GetBytes( "1234" ), 1, 64 );
            // Verify
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0x55, 0x3a, 0x1f, 0xe2, 0xcd, 0xf1, 0x7b, 0xa2, 0xfc, 0x88, 0xac, 0x4a, 0x2b, 0xde, 0xff, 0x05, 
                                             0xb6, 0x7f, 0xa8, 0x2a, 0x4a, 0xd7, 0x61, 0xd6, 0xc9, 0xa9, 0x1c, 0xf9, 0x6c, 0xf1, 0x3a, 0xe6, 
                                             0xb0, 0x29, 0x40, 0x62, 0x2a, 0x8a, 0xf9, 0xee, 0x9b, 0x57, 0x3c, 0x74, 0x3f, 0xe1, 0x70, 0x1a, 
                                             0x2e, 0x02, 0xae, 0xbd, 0xba, 0xb7, 0x2b, 0x0f, 0xad, 0x2e, 0x48, 0x3e, 0x11, 0xd3, 0x7c, 0xe0,
                                         } ) );
        }

        // ~$ printf "tsU&yUaZulAs4eOV1234\xe1\x32\xa0\xc4\xa6\xe2\x49\x48\x2b\x2c\xb7\x4c\x96\x96\xad\xde\xec\x27\x23\x76\x89\xb7\x72\x09\xef\xa5\x6a\x0f\x7b\xe3\x8e\x39\x7c\x69\x2a\x81\x9d\x11\x52\x4e\xfc\xc1\x5b\x98\x3d\x6e\xd1\x6d\x53\xe2\x72\x78\xe1\xac\xe5\xe1\x65\x6e\xc9\x0e\xc9\x1b\x89\x17" | shasum -a 512
        // 4a44ad9020e454cf467f0ad932fd3c93c7c9d5f92f470d7894adf82f7b071185f944be253b16571aa8448b63ff915d7f8b502372e4903bf8f196aa90191677e9
        [ Test ]
        public void DeriveWithSecondIterationReusesResult( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "abcd" ), Encoding.UTF8.GetBytes( "1234" ), 2, 64 );
            // Verify
            Assert.That( derived,
                         Is.EqualTo( new byte[ ]
                                         {
                                             0x4a, 0x44, 0xad, 0x90, 0x20, 0xe4, 0x54, 0xcf, 0x46, 0x7f, 0x0a, 0xd9, 0x32, 0xfd, 0x3c, 0x93,
                                             0xc7, 0xc9, 0xd5, 0xf9, 0x2f, 0x47, 0x0d, 0x78, 0x94, 0xad, 0xf8, 0x2f, 0x7b, 0x07, 0x11, 0x85,
                                             0xf9, 0x44, 0xbe, 0x25, 0x3b, 0x16, 0x57, 0x1a, 0xa8, 0x44, 0x8b, 0x63, 0xff, 0x91, 0x5d, 0x7f,
                                             0x8b, 0x50, 0x23, 0x72, 0xe4, 0x90, 0x3b, 0xf8, 0xf1, 0x96, 0xaa, 0x90, 0x19, 0x16, 0x77, 0xe9,
                                         } ) );
        }
    }
}