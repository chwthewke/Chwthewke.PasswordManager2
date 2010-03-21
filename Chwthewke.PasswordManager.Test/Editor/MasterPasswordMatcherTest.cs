using System;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
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
                                                  Hashes.Sha512Factory );
        }

        [ Test ]
        public void CannotMatchMasterWhenMissingPasswordGenerator( )
        {
            // Setup
            PasswordDigest digest =
                new PasswordDigestBuilder( )
                    .WithMasterPasswordId( Guid.Parse( "656A5218-892E-4666-A3A9-70766C089044" ) );


            // Exercise
            bool masterPasswordMatches = _matcher.MatchMasterPassword( HashWrapperWithSha512Test.Wrap( "masterPassword" ), digest );
            // Verify
            Assert.That( masterPasswordMatches, Is.False );
        }


        [ Test ]
        public void DoNotMatchMasterPasswordIfHashMismatch( )
        {
            // Setup
            SecureString masterPassword = HashWrapperWithSha512Test.Wrap( "mpmp" );
            string generatedPassword = PasswordGenerators.AlphaNumeric.MakePassword( "key1", masterPassword );
            IPasswordDigester digester = new PasswordDigester( Hashes.Sha512Factory, new TimeProvider( ) );
            PasswordDigest digest = digester.Digest( "key1", generatedPassword, default( Guid ),
                                                     PasswordGenerators.AlphaNumeric.Id, string.Empty );
            // Exercise
            bool match = _matcher.MatchMasterPassword( HashWrapperWithSha512Test.Wrap( "omnomnom" ), digest );
            // Verify
            Assert.That( match, Is.False );
        }


        [ Test ]
        public void MatchMasterPassword( )
        {
            // Setup
            SecureString masterPassword = HashWrapperWithSha512Test.Wrap( "mpmp" );
            string generatedPassword = PasswordGenerators.AlphaNumeric.MakePassword( "key1", masterPassword );
            IPasswordDigester digester = new PasswordDigester( Hashes.Sha512Factory, new TimeProvider( ) );
            PasswordDigest digest = digester.Digest( "key1", generatedPassword, default( Guid ),
                                                     PasswordGenerators.AlphaNumeric.Id, string.Empty );
            // Exercise
            bool match = _matcher.MatchMasterPassword( masterPassword, digest );
            // Verify
            Assert.That( match, Is.True );
        }
    }
}