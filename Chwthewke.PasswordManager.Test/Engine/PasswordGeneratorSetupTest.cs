using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordGeneratorSetupTest
    {
        private Mock<IBaseConverter> _converterMock;

        [ SetUp ]
        public void SetUpConverterMock( )
        {
            _converterMock = new Mock<IBaseConverter>( );
        }


        [ Test ]
        public void TestMismatchedLengthsFail( )
        {
            // Setup
            Alphabet symbols50 = new Alphabet( new StringBuilder( ).Append( new char[50] ).ToString( ) );
            _converterMock.Setup( c => c.Base ).Returns( 12 );
            IBaseConverter converter = _converterMock.Object;
            // Exercise
            // Verify
            Assert.That(
                new TestDelegate( ( ) => new PasswordGenerator( default( Guid ), new Sha512( ), converter, symbols50, 8 ) ),
                Throws.InstanceOf( typeof ( ArgumentException ) ) );
        }

        [ Test ]
        public void TestCanGeneratePasswordsWith64BytesEntropy( )
        {
            // Setup
            Alphabet symbols16 = new Alphabet( "0123456789ABCDEF" );
            _converterMock.Setup( c => c.Base ).Returns( 16 );
            _converterMock.Setup( c => c.BytesNeeded( It.IsAny<int>( ) ) ).Returns( 64 );
            // Exercise
            new PasswordGenerator( default( Guid ), new Sha512( ), _converterMock.Object, symbols16, 8 );
            // Verify
        }

        [ Test ]
        public void TestCannotGeneratePasswordsWithArbitraryEntropy( )
        {
            // Setup
            Alphabet symbols16 = new Alphabet( "0123456789ABCDEF" );
            _converterMock.Setup( c => c.Base ).Returns( 16 );
            _converterMock.Setup( c => c.BytesNeeded( It.IsAny<int>( ) ) ).Returns( 65 );
            // Exercise
            Assert.That(
                new TestDelegate(
                    ( ) => new PasswordGenerator( default( Guid ), new Sha512( ), _converterMock.Object, symbols16, 8 ) ),
                Throws.InstanceOf( typeof ( ArgumentException ) ) );
            // Verify
        }

        [ Test ] // this test basically specifies the implementation... is it any good ?
        public void TestGeneratePassword( )
        {
            // Setup
            const string domain = "chwthewke.net";
            const string masterPassword = "m@st3rp@ssw0rd";

            byte[ ] bytes = { 0x01, 0x02 };

            _converterMock.Setup( c => c.Base ).Returns( 92 );
            _converterMock.Setup( c => c.BytesNeeded( It.IsAny<int>( ) ) ).Returns( 10 );
            _converterMock.Setup( c => c.ConvertBytesToDigits( It.IsAny<byte[ ]>( ), It.IsAny<int>( ) ) ).Returns( bytes );

            Alphabet alphabet = Alphabets.Symbols92;

            IPasswordGenerator engine = new PasswordGenerator( default( Guid ), new Sha512( ), _converterMock.Object,
                                                           alphabet, 12 );

            // Exercise
            string password = engine.MakePassword( domain, SecureTest.Wrap( masterPassword ) );

            // Verify
            byte[ ] hash = new Sha512( ).Hash( Encoding.UTF8.GetBytes( PasswordGenerator.Salt + masterPassword + domain ) );
            _converterMock.Verify( c => c.BytesNeeded( 12 ) );
            _converterMock.Verify( c => c.ConvertBytesToDigits( hash, 12 ) );
            Assert.That( password, Is.EquivalentTo( alphabet.ToString( bytes ) ) );
        }
    }
}