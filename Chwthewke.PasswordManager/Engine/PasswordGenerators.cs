using System;

namespace Chwthewke.PasswordManager.Engine
{
    public static class PasswordGenerators
    {
        public static IPasswordGenerator AlphaNumeric
        {
            get { return _alphaNumeric; }
        }

        public static IPasswordGenerator Full
        {
            get { return _full; }
        }

        private static PasswordGenerator Sha512Generator( Guid id, Alphabet alphabet, int passwordLength )
        {
            return new PasswordGenerator( id, Hashes.Sha512Factory, new BigIntegerBaseConverter( alphabet.Length ),
                                          alphabet,
                                          passwordLength );
        }

        private static readonly PasswordGenerator _alphaNumeric =
            Sha512Generator( Guid.Parse( "74728A10-33D4-4245-B7C9-5D72FC424C41" ), Alphabets.Symbols62, 12 );

        private static readonly PasswordGenerator _full =
            Sha512Generator( Guid.Parse( "CCF1451C-4B30-45A4-99B0-D54EC3C3A7EE" ), Alphabets.Symbols92, 10 );
    }
}