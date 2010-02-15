using System;
using System.Security.Cryptography;

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

        private static SHA512 GetHash( )
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