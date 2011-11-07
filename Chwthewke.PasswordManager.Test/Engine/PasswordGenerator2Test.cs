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
            IDerivedKeyFactory derivedKeyFactory = new Pkbdf2DerivedKeyFactory( );
            PasswordMaterializer materializer = PasswordMaterializers.AlphaNumeric;
            const int iterations = 15000;

            PasswordGenerator2 generator = new PasswordGenerator2( derivedKeyFactory, derivedKeyFactory, materializer, 10000, 32 );
            // Exercise
            DerivedPassword derived = generator.Derive( new PasswordRequest( "abcd", "1234".ToSecureString( ), iterations, default( Guid ) ) );
            // Verify
            string expectedPassword =
                materializer.ToString( derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "abcd" ), Encoding.UTF8.GetBytes( "1234" ),
                                                                    iterations, materializer.BytesNeeded ) );

            Assert.That( derived.Password, Is.EqualTo( expectedPassword ) );
        }

        [ Test ]
        public void GeneratorUsesDerivedKeyFactoryToCreateDigest( )
        {
            // Set up
            IDerivedKeyFactory derivedKeyFactory = new Pkbdf2DerivedKeyFactory( );
            PasswordMaterializer materializer = PasswordMaterializers.AlphaNumeric;
            const int iterations = 15000;
            const int digestIterations = 10000;

            PasswordGenerator2 generator = new PasswordGenerator2( derivedKeyFactory, derivedKeyFactory, materializer, digestIterations, 32 );
            // Exercise
            DerivedPassword derived = 
                generator.Derive( new PasswordRequest( "abcd", "1234".ToSecureString( ), iterations, PasswordGenerators2.Full ) );
            // Verify

            byte[ ] expectedHash = derivedKeyFactory.DeriveKey( PasswordGenerator2.DigestSalt, Encoding.UTF8.GetBytes( derived.Password ),
                                                                digestIterations, 32 );
            PasswordDigest2 expectedDigest = new PasswordDigest2( "abcd", expectedHash, iterations, PasswordGenerators2.Full );

            Assert.That( derived.Digest, Is.EqualTo( expectedDigest ) );
        }
    }
}