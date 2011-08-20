using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [TestFixture]
    public class PasswordDigesterTest
    {
        [SetUp]
        public void SetUpPasswordDigester( )
        {
            _timeProviderMock = new Mock<ITimeProvider>( );
            _hashFactory = new Sha512Factory( );
            _digester = new PasswordDigester( _hashFactory, _timeProviderMock.Object );
        }

        [Test]
        public void DigestHashesPasswordWithSalt( )
        {
            // Setup
            const string generatedPassword = "aPassword";
            byte[] expectedHash = _hashFactory.GetHash( )
                .Append( PasswordDigester.DigestSalt, Encoding.UTF8 )
                .Append( generatedPassword, Encoding.UTF8 )
                .GetValue( );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", generatedPassword, default( Guid ), default( Guid ), null,
                                                      "" );
            // Verify
            Assert.That( digest.Hash, Is.EqualTo( expectedHash ) );
        }

        [Test]
        public void DigesterSetsCreationTimeNowWithNullCreationTime( )
        {
            // Setup
            DateTime now = new DateTime( 123456789123456L );
            _timeProviderMock.Setup( p => p.Now ).Returns( now );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", "generatedPassword", default( Guid ), default( Guid ),
                                                      null, "" );
            // Verify
            Assert.That( digest.CreationTime, Is.EqualTo( now ) );
        }

        [Test]
        public void DigesterUsesGivenCreationTimeOnDigest( )
        {
            // Setup
            DateTime now = new DateTime( 123456789123456L );
            _timeProviderMock.Setup( p => p.Now ).Returns( now );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", "generatedPassword", default( Guid ), default( Guid ),
                                                      new DateTime( 123456787654321L ), "" );
            // Verify
            Assert.That( digest.CreationTime, Is.EqualTo( new DateTime( 123456787654321L ) ) );
        }

        [Test]
        public void DigesterSetsModificationTimeNow( )
        {
            // Setup
            DateTime now = new DateTime( 123456789123456L );
            _timeProviderMock.Setup( p => p.Now ).Returns( now );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", "generatedPassword", default( Guid ), default( Guid ),
                                                      new DateTime( 123456787654321L ), "" );
            // Verify
            Assert.That( digest.ModificationTime, Is.EqualTo( now ) );
        }


        [Test]
        public void PasswordDigesterPassesOtherAttributes( )
        {
            // Setup
            const string key = "aKey";
            Guid masterPasswordId = Guid.Parse( "1E22C5D2-8399-42D9-B474-95B7C0FEB5AF" );
            Guid passwordGeneratorId = Guid.Parse( "18E7700B-234C-4389-9A8B-3D3134EC42FA" );
            const string note = "A nonsensical Note";
            // Exercise
            PasswordDigest digest = _digester.Digest( key, "generatedPassword", masterPasswordId, passwordGeneratorId,
                                                      null, note );
            // Verify
            Assert.That( digest.Key, Is.EqualTo( key ) );
            Assert.That( digest.MasterPasswordId, Is.EqualTo( masterPasswordId ) );
            Assert.That( digest.PasswordGeneratorId, Is.EqualTo( passwordGeneratorId ) );
            Assert.That( digest.Note, Is.EqualTo( note ) );
        }

        private IPasswordDigester _digester;
        private Mock<ITimeProvider> _timeProviderMock;
        private Sha512Factory _hashFactory;
    }
}