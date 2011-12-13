using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    [ Ignore ]
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
        public void DeriveWithSingleIteration( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( PasswordGenerator.DigestSalt, Encoding.UTF8.GetBytes( "password" ), 1, 64 );
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

        [ Test ]
        public void DeriveNonAsciiPasswordWithSingleIteration( )
        {
            // Set up
            // Exercise
            byte[ ] derived = _derivedKeyFactory.DeriveKey( PasswordGenerator.DigestSalt, Encoding.UTF8.GetBytes( "pássw0rð" ), 1, 64 );
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
    }
}