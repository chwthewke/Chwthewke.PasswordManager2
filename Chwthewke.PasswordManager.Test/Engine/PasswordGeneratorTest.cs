using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class PasswordGeneratorTest
    {
        [ Test ]
        public void TestMismatchedLengthsFail( )
        {
            // Setup
            Alphabet symbols50 = new Alphabet( new StringBuilder( ).Append( new char[ 50 ] ).ToString( ) );
            IBaseConverter baseConverter = new BigIntegerBaseConverter( 12 );
            // Exercise
            // Verify
            Assert.That(
                new TestDelegate(
                    ( ) => new PasswordGenerator( default( Guid ), new Sha512Factory( ), baseConverter, symbols50, 8 ) ),
                Throws.InstanceOf( typeof ( ArgumentException ) ) );
        }

        [ Test ]
        public void TestCanGeneratePasswordsWith64BytesEntropy( )
        {
            // Setup
            Alphabet symbols16 = new Alphabet( "0123456789ABCDEF" );
            IBaseConverter baseConverter = new BigIntegerBaseConverter( 16 );
            // Exercise
            new PasswordGenerator( default( Guid ), new Sha512Factory( ), baseConverter, symbols16, 128 );
            // Verify
        }

        [ Test ]
        public void TestCannotGeneratePasswordsWithArbitraryEntropy( )
        {
            // Setup
            Alphabet symbols16 = new Alphabet( "0123456789ABCDEF" );
            IBaseConverter baseConverter = new BigIntegerBaseConverter( 16 );
            // Exercise
            Assert.That(
                new TestDelegate(
                    ( ) =>
                    new PasswordGenerator( default( Guid ), new Sha512Factory( ), baseConverter, symbols16, 129 ) ),
                Throws.InstanceOf( typeof ( ArgumentException ) ) );
            // Verify
        }

        [ Test ]
        public void TestGeneratePassword( )
        {
            // Setup
            const string domain = "chwthewke.net";
            const string masterPassword = "m@st3rp@ssw0rd";


            Alphabet alphabet = Alphabets.Symbols92;
            IBaseConverter baseConverter = new BigIntegerBaseConverter( 92 );


            IPasswordGenerator engine =
                new PasswordGenerator( default( Guid ), new Sha512Factory( ), baseConverter, alphabet, 10 );

            // Exercise
            string password = engine.MakePassword( domain, masterPassword.ToSecureString( ) );

            // Verify
            byte[ ] hash =
                new Sha512Factory( ).GetHash( ).Append(
                    Encoding.UTF8.GetBytes( PasswordGenerator.Salt + masterPassword + domain ) )
                    .GetValue( );
            byte[ ] passwordDigits = baseConverter.ConvertBytesToDigits( hash, 10 );
            string expectedPassword = alphabet.ToString( passwordDigits );
            Assert.That( password, Is.EqualTo( expectedPassword ) );
        }
    }
}