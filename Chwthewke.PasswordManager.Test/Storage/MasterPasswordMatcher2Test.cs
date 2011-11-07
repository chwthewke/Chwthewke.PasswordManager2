using System;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class MasterPasswordMatcher2Test
    {
        private readonly Guid _masterPasswordId = Guid.Parse( "DAAB4016-AF5C-4C79-900E-B01E8D771C12" );

// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IMasterPasswordMatcher MasterPasswordMatcher { get; set; }

        public IPasswordDigester Digester { get; set; }

        public IPasswordDatabase PasswordDatabase { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global


        private IPasswordCollection _passwordCollection;
        private IPasswordDerivationEngine _passwordDerivationEngine;
        private IMasterPasswordMatcher _masterPasswordMatcher;

        [ SetUp ]
        public void SetUpStore( )
        {
            _passwordCollection = new PasswordCollection( new InMemoryPasswordData( ) );
            _passwordDerivationEngine = new PasswordDerivationEngine( );
            _masterPasswordMatcher = new MasterPasswordMatcher2( _passwordDerivationEngine, _passwordCollection );
        }

        [ Test ]
        public void FindMasterPasswordInStoreWhenDigestMatches( )
        {
            // Setup
            SecureString masterPassword = "toto".ToSecureString( );

            DerivedPassword match = 
                _passwordDerivationEngine.Derive( new PasswordRequest( "key1", masterPassword, 10, PasswordGenerators2.Full ) );

            PasswordDigestDocument matchingDocument = 
                new PasswordDigestDocument( match.Digest, _masterPasswordId, new DateTime( 2011, 11, 1 ), new DateTime( 2011, 11, 1 ), string.Empty );
            
            _passwordCollection.SavePassword( matchingDocument );

            DerivedPassword nonMatch =
                _passwordDerivationEngine.Derive( new PasswordRequest( "key2", "tata".ToSecureString( ), 1, PasswordGenerators2.AlphaNumeric ) );

            PasswordDigestDocument nonMatchingDocument =
                new PasswordDigestDocument( nonMatch.Digest, Guid.NewGuid( ), new DateTime( 2011, 11, 1 ), new DateTime( 2011, 11, 1 ), string.Empty );

            _passwordCollection.SavePassword( nonMatchingDocument );

            // Exercise
            Guid? guid = _masterPasswordMatcher.IdentifyMasterPassword( masterPassword );
            // Verify
            Assert.That( guid.HasValue, Is.True );
            Assert.That( guid, Is.EqualTo( _masterPasswordId ) );
        }

        [ Test ]
        public void CannotFindMasterPasswordInStoreWhenNoDigestMatches( )
        {
            // Setup
            DerivedPassword nonMatch =
                _passwordDerivationEngine.Derive( new PasswordRequest( "key2", "tata".ToSecureString( ), 1, PasswordGenerators2.AlphaNumeric ) );

            PasswordDigestDocument nonMatchingDocument =
                new PasswordDigestDocument( nonMatch.Digest, Guid.NewGuid( ), new DateTime( 2011, 11, 1 ), new DateTime( 2011, 11, 1 ), string.Empty );

            _passwordCollection.SavePassword( nonMatchingDocument );

            // Exercise
            Guid? guid = _masterPasswordMatcher.IdentifyMasterPassword( "toto".ToSecureString( ) );
            // Verify
            Assert.That( guid.HasValue, Is.False );
        }
    }
}