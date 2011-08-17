using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;
using Autofac;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [TestFixture]
    public class MasterPasswordMatcherTest
    {
        private readonly Guid _masterPasswordId = Guid.Parse( "DAAB4016-AF5C-4C79-900E-B01E8D771C12" );

// ReSharper disable UnusedAutoPropertyAccessor.Global
        public IMasterPasswordMatcher MasterPasswordMatcher { get; set; }

        public IPasswordDigester Digester { get; set; }

        public IPasswordDatabase PasswordDatabase { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        [SetUp]
        public void SetUpStore( )
        {
            AppSetUp.TestContainer( )
                .InjectProperties( this );
        }

        [Test]
        public void FindMasterPasswordInStoreWhenDigestMatches( )
        {
            // Setup
            SecureString masterPassword = "toto".ToSecureString( );
            PasswordDigest matchingDigest =
                Digester.Digest( "key1",
                                 PasswordGenerators.Full.MakePassword( "key1", masterPassword ),
                                 _masterPasswordId,
                                 PasswordGenerators.Full.Id,
                                 new DateTime(), 
                                 string.Empty );
            PasswordDatabase.AddOrUpdate( matchingDigest );

            PasswordDigest notMatchingDigest =
                Digester.Digest( "key2",
                                 PasswordGenerators.Full.MakePassword( "key2", "tata".ToSecureString( ) ),
                                 Guid.NewGuid( ),
                                 PasswordGenerators.Full.Id,
                                 new DateTime( ),
                                 string.Empty );
            PasswordDatabase.AddOrUpdate( notMatchingDigest );
            // Exercise
            Guid? guid = MasterPasswordMatcher.IdentifyMasterPassword( masterPassword );
            // Verify
            Assert.That( guid.HasValue, Is.True );
            Assert.That( guid, Is.EqualTo( _masterPasswordId ) );
        }

        [Test]
        public void CannotFindMasterPasswordInStoreWhenNoDigestMatches( )
        {
            // Setup
            PasswordDigest notMatchingDigest =
                Digester.Digest( "key1",
                                 PasswordGenerators.Full.MakePassword( "key1", "tata".ToSecureString( ) ),
                                 Guid.NewGuid( ),
                                 PasswordGenerators.Full.Id,
                                 new DateTime( ),
                                 string.Empty );
            PasswordDatabase.AddOrUpdate( notMatchingDigest );
            // Exercise
            Guid? guid = MasterPasswordMatcher.IdentifyMasterPassword( "toto".ToSecureString( ) );
            // Verify
            Assert.That( guid.HasValue, Is.False );
        }
    }
}