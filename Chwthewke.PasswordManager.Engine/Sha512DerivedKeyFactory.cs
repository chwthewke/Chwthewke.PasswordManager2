using System;
using System.Security.Cryptography;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal class Sha512DerivedKeyFactory : IDerivedKeyFactory
    {
        public Sha512DerivedKeyFactory( Func<byte[ ], byte[ ], byte[ ]> composePasswordAndSalt )
        {
            _composePasswordAndSalt = composePasswordAndSalt;
        }

        public byte[ ] DeriveKey( byte[ ] salt, byte[ ] password, int iterations, int byteCount )
        {
            if ( byteCount > 64 )
                throw new ArgumentException( "Cannot request more than 64 bytes." );
            if ( iterations <= 0 )
                throw new ArgumentException( "'iterations' must be strictly positive." );


            byte[ ] bytes = salt;
            for ( int i = 0; i < iterations; ++i )
                bytes = DeriveOnce( bytes, password );

            byte[ ] result = new byte[ byteCount ];
            Array.Copy( bytes, result, byteCount );
            return result;
        }

        private byte[ ] DeriveOnce( byte[ ] salt, byte[ ] password )
        {
            HashAlgorithm hashAlgorithm = Sha512Hash( );

            byte[ ] hashSource = _composePasswordAndSalt( salt, password );

            hashAlgorithm.TransformFinalBlock( hashSource, 0, hashSource.Length );

            return hashAlgorithm.Hash;
        }


        private static HashAlgorithm Sha512Hash( )
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

        private readonly Func<byte[ ], byte[ ], byte[ ]> _composePasswordAndSalt;
    }
}