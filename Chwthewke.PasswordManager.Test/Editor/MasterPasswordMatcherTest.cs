using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [TestFixture]
    [Ignore]
    public class MasterPasswordMatcherTest
    {
        [ Test ]
        public void MatchMasterPassword( )
        {
            // Setup
            IPasswordGenerator generator = PasswordGenerators.AlphaNumeric;
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );
            string password = generator.MakePassword( "key1", masterPassword );
            // Exercise
            // Verify
        }
    }
}