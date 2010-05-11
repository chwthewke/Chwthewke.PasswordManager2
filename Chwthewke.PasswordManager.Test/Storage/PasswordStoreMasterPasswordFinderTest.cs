using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordStoreMasterPasswordFinderTest
    {
        private IPasswordStore _store;
        private Guid _masterPasswordId;
        private IPasswordDigester _digester;

        [ SetUp ]
        public void SetUpStore( )
        {
            IHashFactory hashFactory = new Sha512Factory( );
            _store = new PasswordStore( PasswordGenerators.All, hashFactory );
            _digester = new PasswordDigester( hashFactory, new TimeProvider( ) );

            _masterPasswordId = Guid.Parse( "DAAB4016-AF5C-4C79-900E-B01E8D771C12" );
        }

        [ Test ]
        public void FindMasterPasswordInStoreWhenDigestMatches( )
        {
            // Setup
            SecureString masterPassword = "toto".ToSecureString( );
            PasswordDigest matchingDigest =
                _digester.Digest( "key1",
                                  PasswordGenerators.Full.MakePassword( "key1", masterPassword ),
                                  _masterPasswordId,
                                  PasswordGenerators.Full.Id,
                                  string.Empty );
            _store.AddOrUpdate( matchingDigest );

            PasswordDigest notMatchingDigest =
                _digester.Digest( "key2",
                                  PasswordGenerators.Full.MakePassword( "key2", "tata".ToSecureString( ) ),
                                  Guid.NewGuid( ),
                                  PasswordGenerators.Full.Id,
                                  string.Empty );
            _store.AddOrUpdate( notMatchingDigest );
            // Exercise
            Guid? guid = _store.IdentifyMasterPassword( masterPassword );
            // Verify
            Assert.That( guid.HasValue, Is.True );
            Assert.That( guid, Is.EqualTo( _masterPasswordId ) );
        }

        [ Test ]
        public void CannotFindMasterPasswordInStoreWhenNoDigestMatches( )
        {
            // Setup
            PasswordDigest notMatchingDigest =
                _digester.Digest( "key1",
                                  PasswordGenerators.Full.MakePassword( "key1", "tata".ToSecureString( ) ),
                                  Guid.NewGuid( ),
                                  PasswordGenerators.Full.Id,
                                  string.Empty );
            _store.AddOrUpdate( notMatchingDigest );
            // Exercise
            Guid? guid = _store.IdentifyMasterPassword( "toto".ToSecureString( ) );
            // Verify
            Assert.That( guid.HasValue, Is.False );
        }
    }
}