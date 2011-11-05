using System;
using System.Runtime.InteropServices;
using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal static class SecureStringExtensions
    {
        public static unsafe T ConsumeBytes<T>( this SecureString secureString, Encoding encoding, Func<byte[ ], T> consumer )
        {
            IntPtr bstr = default( IntPtr );
            int charCount = secureString.Length;

            try
            {
                bstr = Marshal.SecureStringToBSTR( secureString );

                char* charPtr = (char*) bstr;
                int byteCount = encoding.GetByteCount( charPtr, charCount );

                byte[ ] bytes = new byte[ byteCount ];
                GCHandle handleToBytes = GCHandle.Alloc( bytes, GCHandleType.Pinned );
                try
                {
                    byte* bytePtr = (byte*) Marshal.UnsafeAddrOfPinnedArrayElement( bytes, 0 );
                    encoding.GetBytes( charPtr, charCount, bytePtr, byteCount );

                    return consumer( bytes );
                }
                finally
                {
                    for ( int i = 0; i < byteCount; ++i )
                        bytes[ i ] = 0;

                    handleToBytes.Free( );
                }
            }
            finally
            {
                Marshal.ZeroFreeBSTR( bstr );
            }
        }
    }
}