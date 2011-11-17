using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordGeneratorTest
    {
        [ Test ]
        public void GeneratorUsesDerivedKeyFactoryAndMaterializerToDerivePasswordFromRequest( )
        {
            // Set up
            IDerivedKeyFactory derivedKeyFactory = new Pkbdf2DerivedKeyFactory( 15000 );
            IDerivedKeyFactory digestFactory = new Pkbdf2DerivedKeyFactory( 10000 );
            PasswordMaterializer materializer = PasswordMaterializers.AlphaNumeric;

            PasswordGenerator generator = new PasswordGenerator( derivedKeyFactory, digestFactory, materializer, 32 );
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

            PasswordGenerator generator = new PasswordGenerator( derivedKeyFactory, digestFactory, materializer, 32 );
            // Exercise
            DerivedPassword derived = 
                generator.Derive( new PasswordRequest( "abcd", "1234".ToSecureString( ), 1, PasswordGenerators.Full ) );
            // Verify

            byte[ ] expectedHash = digestFactory.DeriveKey( PasswordGenerator.DigestSalt, Encoding.UTF8.GetBytes( derived.Password ),
                                                                1, 32 );
            PasswordDigest expectedDigest = new PasswordDigest( "abcd", expectedHash, 1, PasswordGenerators.Full );

            Assert.That( derived.Digest, Is.EqualTo( expectedDigest ) );
        }
    }
}