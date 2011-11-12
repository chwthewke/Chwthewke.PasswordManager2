using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordGenerator2Test
    {
        [ Test ]
        public void GeneratorUsesDerivedKeyFactoryAndMaterializerToDerivePasswordFromRequest( )
        {
            // Set up
            IDerivedKeyFactory derivedKeyFactory = new Pkbdf2DerivedKeyFactory( 15000 );
            IDerivedKeyFactory digestFactory = new Pkbdf2DerivedKeyFactory( 10000 );
            PasswordMaterializer materializer = PasswordMaterializers.AlphaNumeric;

            PasswordGenerator2 generator = new PasswordGenerator2( derivedKeyFactory, digestFactory, materializer, 32 );
            // Exercise
            DerivedPassword derived = generator.Derive( new PasswordRequest( "abcd", "1234".ToSecureString( ), 3, default( Guid ) ) );
            // Verify
            string expectedPassword =
                materializer.ToString( derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "abcd" ), Encoding.UTF8.GetBytes( "1234" ),
                                                                    3, materializer.BytesNeeded ) );

            Assert.That( derived.Password, Is.EqualTo( expectedPassword ) );
        }

        [ Test ]
        public void GeneratorUsesDerivedKeyFactoryToCreateDigest( )
        {
            // Set up
            IDerivedKeyFactory derivedKeyFactory = new Pkbdf2DerivedKeyFactory( 15000 );
            IDerivedKeyFactory digestFactory = new Pkbdf2DerivedKeyFactory( 10000 );
            PasswordMaterializer materializer = PasswordMaterializers.AlphaNumeric;

            PasswordGenerator2 generator = new PasswordGenerator2( derivedKeyFactory, digestFactory, materializer, 32 );
            // Exercise
            DerivedPassword derived = 
                generator.Derive( new PasswordRequest( "abcd", "1234".ToSecureString( ), 1, PasswordGenerators2.Full ) );
            // Verify

            byte[ ] expectedHash = digestFactory.DeriveKey( PasswordGenerator2.DigestSalt, Encoding.UTF8.GetBytes( derived.Password ),
                                                                1, 32 );
            PasswordDigest2 expectedDigest = new PasswordDigest2( "abcd", expectedHash, 1, PasswordGenerators2.Full );

            Assert.That( derived.Digest, Is.EqualTo( expectedDigest ) );
        }
    }
}