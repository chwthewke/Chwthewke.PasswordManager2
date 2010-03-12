using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordDigesterTests
    {
        [ SetUp ]
        public void SetUpPasswordDigester( )
        {
            _hashMock = new Mock<IHash>( );
            _timeProviderMock = new Mock<ITimeProvider>( );
            _digester = new PasswordDigester( _hashMock.Object, _timeProviderMock.Object );
        }

        [ Test ]
        public void DigestHashesPasswordWithSalt( )
        {
            // Setup
            const string generatedPassword = "aPassword";
            byte[ ] bytesToHash = Encoding.UTF8.GetBytes( PasswordDigester.DigestSalt + generatedPassword );
            byte[ ] fakeHash = new byte[ ] { 0x50, 0x51, 0x52 };
            _hashMock.Setup( h => h.Hash( bytesToHash ) ).Returns( fakeHash );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", generatedPassword, default( Guid ), default( Guid ), "" );
            // Verify
            _hashMock.Verify( h => h.Hash( bytesToHash ) );
            Assert.That( digest.Hash, Is.EqualTo( fakeHash ) );
        }

        [ Test ]
        public void DigesterSetsCurrentTimeOnDigest( )
        {
            // Setup
            DateTime creationTime = new DateTime( 123456789123456L );
            _timeProviderMock.Setup( tpm => tpm.Now ).Returns( creationTime );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", "generatedPassword", default( Guid ), default( Guid ), "" );
            // Verify
            Assert.That( digest.CreationTime, Is.EqualTo( creationTime ) );
        }


        [ Test ]
        public void PasswordDigesterPassesOtherAttributes( )
        {
            // Setup
            string key = "aKey";
            Guid masterPasswordId = Guid.Parse( "1E22C5D2-8399-42D9-B474-95B7C0FEB5AF" );
            Guid passwordGeneratorId = Guid.Parse( "18E7700B-234C-4389-9A8B-3D3134EC42FA" );
            string note = "A nonsensical Note";
            // Exercise
            PasswordDigest digest = _digester.Digest( key, "generatedPassword", masterPasswordId, passwordGeneratorId,
                                                      note );
            // Verify
            Assert.That( digest.Key, Is.EqualTo( key ) );
            Assert.That( digest.MasterPasswordId, Is.EqualTo( masterPasswordId ) );
            Assert.That( digest.PasswordGeneratorId, Is.EqualTo( passwordGeneratorId ) );
            Assert.That( digest.Note, Is.EqualTo( note ) );
        }

        private PasswordDigester _digester;

        private Mock<IHash> _hashMock;
        private Mock<ITimeProvider> _timeProviderMock;
    }
}