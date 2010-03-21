using System;
using System.Security.Cryptography;

namespace Chwthewke.PasswordManager.Engine
{
    internal class Sha512Factory : IHashFactory
    {
        public IHash GetHash( )
        {
            return new HashWrapper( GetSha512HashAlgorithm( ) );
        }

        public int HashSize
        {
            get { return 64; }
        }

        public static SHA512 GetSha512HashAlgorithm( )
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