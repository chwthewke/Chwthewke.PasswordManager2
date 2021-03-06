using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App
{
    [ TestFixture ]
    public class ResourcesTest
    {
        [ Test ]
        public void TestPasswordGeneratorNames( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That(
                Resources.ResourceManager.GetString( "PasswordGenerator" + PasswordGenerators.Full.ToString( "N" ) ),
                Is.EqualTo( "Complex" ) );
            Assert.That(
                Resources.ResourceManager.GetString( "PasswordGenerator" + PasswordGenerators.AlphaNumeric.ToString( "N" ) ),
                Is.EqualTo( "Alphanumeric" ) );
        }
    }
}