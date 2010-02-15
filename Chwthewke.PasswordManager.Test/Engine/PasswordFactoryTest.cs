using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordFactoryTest
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
            Symbols symbols50 = new Symbols( new StringBuilder( ).Append( new char[50] ).ToString( ) );
            _converterMock.Setup( c => c.Base ).Returns( 12 );
            IBaseConverter converter = _converterMock.Object;
            // Exercise
            // Verify
            Assert.That( new TestDelegate( ( ) => new PasswordFactory( new Sha512( ), converter, symbols50, 8 ) ),
                         Throws.InstanceOf( typeof ( ArgumentException ) ) );
        }

        [ Test ]
        public void TestCanGeneratePasswordsWith64BytesEntropy( )
        {
            // Setup
            Symbols symbols16 = new Symbols( "0123456789ABCDEF" );
            _converterMock.Setup( c => c.Base ).Returns( 16 );
            _converterMock.Setup( c => c.UsedBytes( It.IsAny<int>( ) ) ).Returns( 64 );
            // Exercise
            new PasswordFactory( new Sha512( ), _converterMock.Object, symbols16, 8 );
            // Verify
        }

        [ Test ]
        public void TestCannotGeneratePasswordsWithArbitraryEntropy( )
        {
            // Setup
            Symbols symbols16 = new Symbols( "0123456789ABCDEF" );
            _converterMock.Setup( c => c.Base ).Returns( 16 );
            _converterMock.Setup( c => c.UsedBytes( It.IsAny<int>( ) ) ).Returns( 65 );
            // Exercise
            Assert.That(
                new TestDelegate( ( ) => new PasswordFactory( new Sha512( ), _converterMock.Object, symbols16, 8 ) ),
                Throws.InstanceOf( typeof ( ArgumentException ) ) );
            // Verify
        }

        [ Test ]
        public void TestCannotRequestPasswordsLessThan8CharsLong( )
        {
            // Setup
            Symbols symbols16 = new Symbols( "0123456789ABCDEF" );
            _converterMock.Setup( c => c.Base ).Returns( 16 );
            _converterMock.Setup( c => c.UsedBytes( It.IsAny<int>( ) ) ).Returns( 64 );
            // Exercise
            Assert.That(
                new TestDelegate( ( ) => new PasswordFactory( new Sha512( ), _converterMock.Object, symbols16, 7 ) ),
                Throws.InstanceOf( typeof ( ArgumentException ) ) );
            // Verify
        }

        [ Test ] // this test basically specifies the implementation... is it any good ?
        public void TestGeneratePassword( )
        {
            // Setup
            const string domain = "google.com";
            const string masterPassword = "m@st3rp@ssw0rd";

            byte[ ] bytes = { 0x01, 0x02 };

            _converterMock.Setup( c => c.Base ).Returns( 92 );
            _converterMock.Setup( c => c.UsedBytes( It.IsAny<int>( ) ) ).Returns( 10 );
            _converterMock.Setup( c => c.Convert( It.IsAny<byte[ ]>( ), It.IsAny<int>( ) ) ).Returns( bytes );

            Symbols symbols = Symbols.Symbols92;

            PasswordFactory engine = new PasswordFactory( new Sha512( ), _converterMock.Object, symbols, 12 );

            // Exercise
            string password = engine.MakePassword( domain, SecureTest.Wrap( masterPassword ) );

            // Verify
            byte[ ] hash = new Sha512( ).Hash(  Encoding.UTF8.GetBytes( PasswordFactory.Salt + masterPassword + domain ) );
            _converterMock.Verify( c => c.UsedBytes( 12 ) );
            _converterMock.Verify( c => c.Convert( hash, 12 ) );
            Assert.That( password, Is.EquivalentTo( symbols.ToString( bytes ) ) );
        }
    }
}