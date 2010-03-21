using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordDigesterTests
    {
        [ SetUp ]
        public void SetUpPasswordDigester( )
        {
            _hashMock = new Mock<IHash2>( );
            var hashFactoryMock = new Mock<IHashFactory>( );
            
            hashFactoryMock.Setup( f => f.GetHash(  ) ).Returns( _hashMock.Object );
            _hashMock.Setup( h => h.Append( It.IsAny<string>( ), It.IsAny<Encoding>( ) ) ).Returns( _hashMock.Object );

            _timeProviderMock = new Mock<ITimeProvider>( );
            _digester = new PasswordDigester( hashFactoryMock.Object, _timeProviderMock.Object );
        }

        [ Test ]
        public void DigestHashesPasswordWithSalt( )
        {
            // Setup
            const string generatedPassword = "aPassword";
            byte[ ] fakeHash = new byte[ ] { 0x50, 0x51, 0x52 };
            _hashMock.Setup( h => h.GetValue(  ) ).Returns( fakeHash );
            // Exercise
            PasswordDigest digest = _digester.Digest( "aKey", generatedPassword, default( Guid ), default( Guid ), "" );
            // Verify
            _hashMock.Verify( h => h.Append( PasswordDigester.DigestSalt, Encoding.UTF8 ) );
            _hashMock.Verify( h => h.Append( generatedPassword, Encoding.UTF8 ) );
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

        private IPasswordDigester _digester;

        private Mock<IHash2> _hashMock;
        private Mock<ITimeProvider> _timeProviderMock;
    }
}