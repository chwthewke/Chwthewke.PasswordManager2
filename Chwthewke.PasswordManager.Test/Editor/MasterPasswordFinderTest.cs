using System;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class MasterPasswordFinderTest
    {
        private IPasswordStore _store;
        private Mock<IMasterPasswordMatcher> _matcherMock;
        private Guid _masterPasswordId;

        [ SetUp ]
        public void SetUpStore( )
        {
            _store = new PasswordStore( );
            _masterPasswordId = Guid.Parse( "DAAB4016-AF5C-4C79-900E-B01E8D771C12" );
            _store.AddOrUpdate( new PasswordDigest( "key1", new byte[0], _masterPasswordId, default( Guid ),
                                                    new DateTime( ),
                                                    string.Empty ) );
            _store.AddOrUpdate( new PasswordDigest( "key2", new byte[0],
                                                    Guid.Parse( "88D42578-664E-43E0-986E-816E6BFC2562" ),
                                                    default( Guid ), new DateTime( ),
                                                    string.Empty ) );
            _matcherMock = new Mock<IMasterPasswordMatcher>( );
        }

        [ Test ]
        public void FindMasterPasswordInStoreWhenDigestMatches( )
        {
            // Setup
            IMasterPasswordFinder finder = new MasterPasswordFinder( _store, _matcherMock.Object );
            _matcherMock.Setup( m => m.MatchMasterPassword( It.IsAny<SecureString>( ), It.IsAny<PasswordDigest>( ) ) )
                .Returns( ( SecureString s, PasswordDigest d ) => d.MasterPasswordId == _masterPasswordId );
            // Exercise
            Guid? guid = finder.IdentifyMasterPassword( SecureTest.Wrap( "toto" ) );
            // Verify
            Assert.That( guid.HasValue, Is.True );
            Assert.That( guid, Is.EqualTo( _masterPasswordId ) );
        }

        [Test]
        public void CannotFindMasterPasswordInStoreWhenNoDigestMatches( )
        {
            // Setup
            IMasterPasswordFinder finder = new MasterPasswordFinder( _store, _matcherMock.Object );
            _matcherMock.Setup( m => m.MatchMasterPassword( It.IsAny<SecureString>( ), It.IsAny<PasswordDigest>( ) ) )
                .Returns( false );
            // Exercise
            Guid? guid = finder.IdentifyMasterPassword( SecureTest.Wrap( "toto" ) );
            // Verify
            Assert.That( guid.HasValue, Is.False );
        }


    }
}