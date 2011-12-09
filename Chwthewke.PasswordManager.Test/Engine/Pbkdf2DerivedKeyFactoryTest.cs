using System;
using System.Collections.Generic;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class Pbkdf2DerivedKeyFactoryTest
    {
        [ Test ]
        [ Category( "Long" ) ]
        public void TimingTest( )
        {
            // Set up
            var derivedKeyFactory = new Pbkdf2DerivedKeyFactory( 1000 );
            int result = 0;
            // Exercise
            for ( int i = 0; i < 100; ++i )
            {
                var derived = derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "abcd" + i ),
                                                           Encoding.UTF8.GetBytes( "1234" ), 1, 10 );
                result += derived.Length;
            }
            // Verify
            Console.WriteLine( @"result: {0}", result );
        }

        [Test]
        public void RegressionTest( )
        {
            // Set up
            var derivedKeyFactory = new Pbkdf2DerivedKeyFactory( 1000 );

            byte[ ] expectedBytes = new byte[ ]
                                        {
                                            0xab, 0x46, 0x96, 0x84, 0x67, 0x78, 0xfc, 0x90, 
                                            0x2a, 0x97, 0xab, 0xd7, 0x74, 0x93, 0x7c, 0x4a, 
                                            0x80, 0x6e, 0x88, 0xd6, 0x54, 0x39, 0x50, 0x0f, 
                                            0xf4, 0xd8, 0x56, 0x5d, 0x62, 0x1f, 0xbb, 0xe8
                                        };
            byte[ ] variableSalt = new byte[ ] { 0x19, 0x2a, 0x3b, 0x4c, 0x5d, 0x6e, 0x7f, 0x80 };

            // Exercise
            byte[ ] result = derivedKeyFactory.DeriveKey(
                variableSalt,
                Encoding.UTF8.GetBytes( "passw0rd" ),
                0, 32 );
            // Verify

            Assert.That( result, Is.EquivalentTo( expectedBytes ) );
        }
        [Test]
        public void RegressionTestWithNonAsciiPassword( )
        {
            // Set up
            var derivedKeyFactory = new Pbkdf2DerivedKeyFactory( 1000 );

            byte[ ] expectedBytes = new byte[ ]
                                        {
                                            0x94, 0xeb, 0x82, 0xf8, 0x15, 0xee, 0x4c, 0xd6, 
                                            0x13, 0xe9, 0xb2, 0x1a, 0x35, 0xdd, 0x23, 0x87, 
                                            0x57, 0xdb, 0x57, 0x28, 0xf4, 0x34, 0x2c, 0xc6, 
                                            0xa1, 0xb2, 0x98, 0x90, 0x0c, 0x13, 0x58, 0xf4
                                        };

            byte[ ] variableSalt = new byte[ ] { 0x19, 0x2a, 0x3b, 0x4c, 0x5d, 0x6e, 0x7f, 0x80 };

            // Exercise
            byte[ ] result = derivedKeyFactory.DeriveKey(
                variableSalt,
                Encoding.UTF8.GetBytes( "pässw0rð" ),
                0, 32 );
            // Verify

            Assert.That( result, Is.EquivalentTo( expectedBytes ) );
        }
    }
}