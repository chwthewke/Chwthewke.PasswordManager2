using System;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class MasterPasswordMatcherTest
    {
        private IMasterPasswordMatcher _matcher;

        [ SetUp ]
        public void SetUpMasterPasswordMatcher( )
        {
            _matcher = new MasterPasswordMatcher( new[ ] { PasswordGenerators.AlphaNumeric, PasswordGenerators.Full },
                                                  new Sha512( ) );
        }

        [ Test ]
        public void CannotMatchMasterWhenMissingPasswordGenerator( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigest( "aKey", new byte[ ] { 0xda }, default( Guid ),
                                                        Guid.Parse( "656A5218-892E-4666-A3A9-70766C089044" ),
                                                        new DateTime( ), string.Empty );
            // Exercise
            bool masterPasswordMatches = _matcher.MatchMasterPassword( SecureTest.Wrap( "masterPassword" ), digest );
            // Verify
            Assert.That( masterPasswordMatches, Is.False );
        }


        [ Test ]
        public void DoNotMatchMasterPasswordIfHashMismatch( )
        {
            // Setup
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );
            string generatedPassword = PasswordGenerators.AlphaNumeric.MakePassword( "key1", masterPassword );
            PasswordDigester digester = new PasswordDigester( new Sha512( ), new TimeProvider( ) );
            PasswordDigest digest = digester.Digest( "key1", generatedPassword, default( Guid ),
                                                     PasswordGenerators.AlphaNumeric.Id, string.Empty );
            // Exercise
            bool match = _matcher.MatchMasterPassword( SecureTest.Wrap( "omnomnom" ), digest );
            // Verify
            Assert.That( match, Is.False );
        }


        [ Test ]
        public void MatchMasterPassword( )
        {
            // Setup
            SecureString masterPassword = SecureTest.Wrap( "mpmp" );
            string generatedPassword = PasswordGenerators.AlphaNumeric.MakePassword( "key1", masterPassword );
            PasswordDigester digester = new PasswordDigester( new Sha512( ), new TimeProvider( ) );
            PasswordDigest digest = digester.Digest( "key1", generatedPassword, default( Guid ),
                                                     PasswordGenerators.AlphaNumeric.Id, string.Empty );
            // Exercise
            bool match = _matcher.MatchMasterPassword( masterPassword, digest );
            // Verify
            Assert.That( match, Is.True );
        }
    }
}