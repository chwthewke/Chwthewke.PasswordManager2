using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class Sha512Test
    {
        [ Test ]
        public void TestHashSomeString( )
        {
            byte[ ] input = Encoding.UTF8.GetBytes( "tatataozornetozal158ce9z52zlll" );


            byte[ ] expected = new byte[ ]
                                   {
                                       0x3e, 0x79, 0xa5, 0x21, 0x8d, 0xb2, 0x61, 0xb7, 0x84, 0xd2, 0x83, 0x67, 0x75,
                                       0xaf, 0xc0, 0x7f, 0x9d, 0x0b, 0xe8, 0x55, 0xbf, 0x8a, 0x7c, 0xcb, 0xa9, 0x98,
                                       0x22, 0x38, 0x1b, 0x61, 0x78, 0x39, 0x4a, 0x2a, 0xf2, 0x57, 0x23, 0xd8, 0x51,
                                       0x93, 0x46, 0xed, 0x56, 0xe7, 0xb3, 0xe9, 0x8b, 0x67, 0x87, 0x70, 0x7f, 0x1f,
                                       0xc4, 0xf3, 0x70, 0xc3, 0x77, 0x17, 0xea, 0xf7, 0xe1, 0x03, 0x78, 0x91,
                                   };

            byte[ ] hash = new Sha512( ).Hash( input );
            Assert.That( hash.Length, Is.EqualTo( 64 ) );
            Assert.That( hash, Is.EquivalentTo( expected ) );
        }

        [ Test ]
        public void TestHashNull( )
        {
            Assert.That( new TestDelegate( ( ) => new Sha512( ).Hash( null ) ),
                         Throws.InstanceOf( typeof( ArgumentException ) ) );
        }
    }
}