using System;
using System.Security.Cryptography;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    public static class Sha512
    {
        public static int Size { get { return 64; } }

        public static byte[ ] Hash( byte[ ] bytes )
        {
            if ( bytes == null )
                throw new ArgumentNullException( "bytes", "Argument cannot be null." );

            SHA512 hash;
            try
            {
                hash = new SHA512Cng( );
            }
            catch ( PlatformNotSupportedException )
            {
                hash = SHA512.Create( );
            }
            return hash.ComputeHash( bytes );
        }

        public static byte[ ] Hash( string str )
        {
            if ( str == null )
                throw new ArgumentNullException( "str", "Argument cannot be null." );

            byte[ ] bytes = null;
            try
            {
                bytes = Encoding.UTF8.GetBytes( str );
                return Hash( bytes );
            }
            finally
            {
                if ( bytes != null )
                    Array.Clear( bytes, 0, bytes.Length );
            }
        }
    }
}