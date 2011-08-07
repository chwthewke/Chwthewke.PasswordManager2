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
        private IMasterPasswordMatcher _masterPasswordMatcher;

        private IPasswordRepository _repository;
        private Guid _masterPasswordId;
        private IPasswordDigester _digester;

        [ SetUp ]
        public void SetUpStore( )
        {
            IHashFactory hashFactory = new Sha512Factory( );
            _repository = new PasswordRepository(  );
            _masterPasswordMatcher = new MasterPasswordMatcher( PasswordGenerators.All, hashFactory, _repository );
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
            _repository.AddOrUpdate( matchingDigest );

            PasswordDigest notMatchingDigest =
                _digester.Digest( "key2",
                                  PasswordGenerators.Full.MakePassword( "key2", "tata".ToSecureString( ) ),
                                  Guid.NewGuid( ),
                                  PasswordGenerators.Full.Id,
                                  string.Empty );
            _repository.AddOrUpdate( notMatchingDigest );
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
            PasswordDigest notMatchingDigest =
                _digester.Digest( "key1",
                                  PasswordGenerators.Full.MakePassword( "key1", "tata".ToSecureString( ) ),
                                  Guid.NewGuid( ),
                                  PasswordGenerators.Full.Id,
                                  string.Empty );
            _repository.AddOrUpdate( notMatchingDigest );
            // Exercise
            Guid? guid = _masterPasswordMatcher.IdentifyMasterPassword( "toto".ToSecureString( ) );
            // Verify
            Assert.That( guid.HasValue, Is.False );
        }
    }
}