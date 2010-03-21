using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Cryptography;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    public class HashWrapper : IHash
    {
        public HashWrapper( HashAlgorithm hashAlgorithm )
        {
            if ( hashAlgorithm == null )
                throw new ArgumentNullException( "hashAlgorithm" );
            _hashAlgorithm = hashAlgorithm;
        }

        public IHash Append( byte[ ] bytes )
        {
            _hashAlgorithm.TransformBlock( bytes, 0, bytes.Length, null, 0 );
            return this;
        }

        public IHash Append( string str, Encoding encoding )
        {
            if ( encoding == null )
                throw new ArgumentNullException( "encoding" );
            return Append( encoding.GetBytes( str ) );
        }

        public IHash Append( SecureString secureString, Encoding encoding )
        {
            if ( secureString == null )
                throw new ArgumentNullException( "secureString" );
            if ( encoding == null )
                throw new ArgumentNullException( "encoding" );

            IntPtr bstr = default( IntPtr );
            int charCount = secureString.Length;

            try
            {
                bstr = Marshal.SecureStringToBSTR( secureString );
                AppendBstr( bstr, charCount, encoding );
            }
            finally
            {
                Marshal.ZeroFreeBSTR( bstr );
            }
            return this;
        }

        private unsafe void AppendBstr( IntPtr bstr, int charCount, Encoding encoding )
        {
            char* charPtr = ( char* ) bstr;
            int byteCount = encoding.GetByteCount( charPtr, charCount );
            byte[ ] bytes = new byte[byteCount];
            GCHandle handleToBytes = GCHandle.Alloc( bytes, GCHandleType.Pinned );

            try
            {
                byte* bytePtr = ( byte* ) Marshal.UnsafeAddrOfPinnedArrayElement( bytes, 0 );

                encoding.GetBytes( charPtr, charCount, bytePtr, byteCount );
                _hashAlgorithm.TransformBlock( bytes, 0, byteCount, null, 0 );
            }
            finally
            {
                for ( int i = 0; i < byteCount; ++i )
                    bytes[ i ] = 0;

                handleToBytes.Free( );
            }
        }

        public byte[ ] GetValue( )
        {
            _hashAlgorithm.TransformFinalBlock( new byte[0], 0, 0 );
            return _hashAlgorithm.Hash;
        }

        private readonly HashAlgorithm _hashAlgorithm;
    }
}