using System.Collections.Generic;
using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class HashWrapperWithSha512Test
    {
        [ SetUp ]
        public void SetUpHashWrapper( )
        {
            _hashWrapper = new HashWrapper( Sha512Factory.GetSha512HashAlgorithm( ) );
        }

        [ Test ]
        public void AppendStringOnceAndGetHash( )
        {
            // Setup

            // Exercise
            _hashWrapper.Append( Input, Encoding.UTF8 );
            // Verify
            Assert.That( _hashWrapper.GetValue( ), Is.EquivalentTo( _output ) );
        }

        [ Test ]
        public void AppendBytesOnceAndGetHash( )
        {
            // Setup

            // Exercise
            _hashWrapper.Append( Encoding.UTF8.GetBytes( Input ) );
            // Verify
            Assert.That( _hashWrapper.GetValue( ), Is.EquivalentTo( _output ) );
        }

        [ Test ]
        public void AppendStringTwiceAndGetHash( )
        {
            // Setup

            // Exercise
            _hashWrapper.Append( Input.Substring( 0, 20 ), Encoding.UTF8 );
            _hashWrapper.Append( Input.Substring( 20, 10 ), Encoding.UTF8 );
            // Verify
            Assert.That( _hashWrapper.GetValue( ), Is.EquivalentTo( _output ) );
        }

        [ Test ]
        public void AppendStringAndBytesAndGetHash( )
        {
            // Setup

            // Exercise
            _hashWrapper.Append( Input.Substring( 0, 20 ), Encoding.UTF8 );
            _hashWrapper.Append( Encoding.UTF8.GetBytes( Input.Substring( 20, 10 ) ) );
            // Verify
            Assert.That( _hashWrapper.GetValue( ), Is.EquivalentTo( _output ) );
        }

        [ Test ]
        public void AppendSecureStringAndGetHash( )
        {
            // Setup
            // Exercise
            _hashWrapper.Append( Wrap( Input ), Encoding.UTF8 );
            // Verify
            Assert.That( _hashWrapper.GetValue( ), Is.EquivalentTo( _output ) );
        }


        internal static SecureString Wrap( IEnumerable<char> s )
        {
            SecureString result = new SecureString( );
            foreach ( char c in s )
                result.AppendChar( c );
            return result;
        }

        private HashWrapper _hashWrapper;

        private const string Input = "tatataozornetozal158ce9z52zlll";


        private readonly byte[ ] _output = new byte[ ]
                                               {
                                                   0x3e, 0x79, 0xa5, 0x21, 0x8d, 0xb2, 0x61, 0xb7, 0x84, 0xd2, 0x83,
                                                   0x67, 0x75, 0xaf, 0xc0, 0x7f, 0x9d, 0x0b, 0xe8, 0x55, 0xbf, 0x8a,
                                                   0x7c, 0xcb, 0xa9, 0x98, 0x22, 0x38, 0x1b, 0x61, 0x78, 0x39, 0x4a,
                                                   0x2a, 0xf2, 0x57, 0x23, 0xd8, 0x51, 0x93, 0x46, 0xed, 0x56, 0xe7,
                                                   0xb3, 0xe9, 0x8b, 0x67, 0x87, 0x70, 0x7f, 0x1f, 0xc4, 0xf3, 0x70,
                                                   0xc3, 0x77, 0x17, 0xea, 0xf7, 0xe1, 0x03, 0x78, 0x91,
                                               };
    }
}