using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    public static class Secure
    {
        /// <summary>
        /// Copies the content of a SecureString to a byte array, encoded with the specified encoding. 
        /// No other copy of the content is left in memory ; in particular no string object is initialized with the content.
        /// The caller should still zero-out the returned array after use.
        /// </summary>
        /// <param name="encoding">the encoding to use for the output.</param>
        /// <param name="secureString">the SecureString to copy</param>
        /// <returns>a byte array</returns>
        public static unsafe byte[ ] GetBytes( Encoding encoding, SecureString secureString )
        {
            if ( encoding == null )
                throw new ArgumentNullException( "encoding" );

            int charCount = secureString.Length;
            IntPtr bstr = Marshal.SecureStringToBSTR( secureString );


            try
            {
                char* charPtr = (char*) bstr;
                int byteCount = Encoding.UTF8.GetByteCount( charPtr, charCount );
                byte[ ] bytes = new byte[byteCount];
                byte* bytePtr = (byte*) Marshal.UnsafeAddrOfPinnedArrayElement( bytes, 0 );
                Encoding.UTF8.GetBytes( charPtr, charCount, bytePtr, byteCount );
                return bytes;
            }
            finally
            {
                Marshal.ZeroFreeBSTR( bstr );
            }
        }

        // TODO : IDisposable wrapper of byte[] ?
    }
}