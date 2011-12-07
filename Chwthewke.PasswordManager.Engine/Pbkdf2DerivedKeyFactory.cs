using System.Linq;
using System.Security.Cryptography;

namespace Chwthewke.PasswordManager.Engine
{
    internal class Pbkdf2DerivedKeyFactory : IDerivedKeyFactory
    {
        public Pbkdf2DerivedKeyFactory( int baseIterations )
        {
            _baseIterations = baseIterations;
        }

        private readonly int _baseIterations;

        public byte[ ] DeriveKey( byte[ ] salt, byte[ ] password, int iterations, int byteCount )
        {
            byte[ ] combinedSalt = InternalSalt.Concat( salt ).ToArray( );
            return new Rfc2898DeriveBytes( password, combinedSalt, _baseIterations + iterations ).GetBytes( byteCount );
        }

        internal static readonly byte[ ] InternalSalt = new byte[ ]
                                                            {
                                                                0x78, 0x94, 0xad, 0xf8, 0x2f, 0x7b, 0x07, 0x11,
                                                                0x85, 0xf9, 0x44, 0xbe, 0x25, 0x3b, 0x16, 0x57,
                                                            };
    }
}