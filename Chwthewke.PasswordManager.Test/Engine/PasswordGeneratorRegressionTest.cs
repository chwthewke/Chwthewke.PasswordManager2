using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordGeneratorRegressionTest
    {
        private const string Key = "chwthewke";
        private const string Password = "m@st3rP4ssw0rD!";

        [ Test ]
        public void GenerateSamplePasswordWithAlphanumeric( )
        {
            // Setup

            // Exercise
            var generatedPassword =
                PasswordGenerators.AlphaNumeric.MakePassword( Key, Password.ToSecureString( ) );
            // Verify
            Assert.That( generatedPassword, Is.EqualTo( "deDYrBiXvMHN" ) );
        }

        [ Test ]
        public void GenerateSamplePasswordWithFull( )
        {
            // Setup

            // Exercise
            var generatedPassword =
                PasswordGenerators.Full.MakePassword( Key, Password.ToSecureString( ) );
            // Verify
            Assert.That( generatedPassword, Is.EqualTo( "(Z'?6G3(w(" ) );
        }
    }
}