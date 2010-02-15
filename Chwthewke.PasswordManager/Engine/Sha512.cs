using System;
using System.Security.Cryptography;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    public class Sha512 : IHash
    {
        public int Size
        {
            get { return 64; }
        }

        public byte[ ] Hash( byte[ ] bytes )
        {
            if ( bytes == null )
                throw new ArgumentNullException( "bytes", "Argument cannot be null." );

            using ( SHA512 hash = GetHash( ) )
                return hash.ComputeHash( bytes );
        }


        public byte[ ] Hash( string str )
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

        private SHA512 GetHash( )
        {
            SHA512 hash;
            try
            {
                hash = new SHA512Cng( );
            }
            catch ( PlatformNotSupportedException )
            {
                hash = SHA512.Create( );
            }
            return hash;
        }
    }
}