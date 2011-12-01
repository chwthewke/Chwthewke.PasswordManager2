using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordDerivationEngineTest
    {
        private const string Key = "chwthewke";
        private const string Password = "m@st3rP4ssw0rD!";

        private IPasswordDerivationEngine _engine;

        [ SetUp ]
        public void SetUpEngine( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators.Generators );
        }

        [ Test ]
        public void GenerateSamplePasswordWithAlphanumeric( )
        {
            // Setup

            // Exercise
            var generatedPassword =
                _engine.Derive( new PasswordRequest( Key, Password.ToSecureString( ), 1, PasswordGenerators.LegacyAlphaNumeric ) ).Password;
            // Verify
            Assert.That( generatedPassword, Is.EqualTo( "deDYrBiXvMHN" ) );
        }

        [ Test ]
        public void GenerateSamplePasswordWithFull( )
        {
            // Setup

            // Exercise
            var generatedPassword =
                _engine.Derive( new PasswordRequest( Key, Password.ToSecureString( ), 1, PasswordGenerators.LegacyFull ) ).Password;
            // Verify
            Assert.That( generatedPassword, Is.EqualTo( "(Z'?6G3(w(" ) );
        }

        [ Test ]
        public void GenerateIteratedPasswordWithFull( )
        {
            // Setup

            // Exercise
            var generatedPassword =
                _engine.Derive( new PasswordRequest( Key, Password.ToSecureString( ), 3, PasswordGenerators.LegacyFull ) ).Password;
            // Verify
            Assert.That( generatedPassword, Is.EqualTo( @"i)?-'@}>`-" ) );
        }

        [ Test ]
        public void GenerateSamplePasswordAndDigestWithAlphanumeric( )
        {
            // Setup
            // Exercise
            var generatedPassword =
                _engine.Derive( new PasswordRequest( Key, Password.ToSecureString( ), 1, PasswordGenerators.LegacyAlphaNumeric ) );
            // Verify

            Assert.That( generatedPassword.Password, Is.EqualTo( "deDYrBiXvMHN" ) );
            PasswordDigest expectedDigest = new PasswordDigest( Key,
                                                                new byte[ ]
                                                                    {
                                                                        0x0E, 0x4A, 0xB3, 0xB3, 0xE5, 0xF6, 0x6F, 0xCF,
                                                                        0x56, 0xCE, 0xE1, 0x14, 0x41, 0x0A, 0x2A, 0x5B,
                                                                        0xDB, 0xC8, 0x72, 0xCC, 0xE3, 0x5D, 0x05, 0x3C,
                                                                        0x20, 0x48, 0x2F, 0x75, 0x8A, 0xF7, 0x05, 0xC7,
                                                                        0xF8, 0xC6, 0x7B, 0x54, 0x87, 0x25, 0xDC, 0xE0,
                                                                        0xC4, 0x0F, 0x3F, 0x80, 0x71, 0x31, 0x2C, 0x19,
                                                                        0xCA, 0x63, 0x29, 0xD9, 0x17, 0xFE, 0x42, 0x2B,
                                                                        0x35, 0x8D, 0xC8, 0xF9, 0x46, 0x63, 0xF0, 0x58,
                                                                    },
                                                                1, PasswordGenerators.LegacyAlphaNumeric );
            Assert.That( generatedPassword.Digest, Is.EqualTo( expectedDigest ) );
        }

        [ Test ]
        public void DerivationEngineUsesSpecifiedPasswordGeneratorToProduce( )
        {
            // Set up

            PasswordRequest request = new PasswordRequest( Key, Password.ToSecureString( ), 1, PasswordGenerators.LegacyFull );
            // Exercise
            IDerivedPassword generatedPassword = _engine.Derive( request );
            // Verify
            DerivedPassword expected = PasswordGenerators.Generators[ PasswordGenerators.LegacyFull ].Derive( request );
            Assert.That( generatedPassword.Password, Is.EqualTo( expected.Password ) );
            Assert.That( generatedPassword.Digest, Is.EqualTo( expected.Digest ) );
        }
    }
}