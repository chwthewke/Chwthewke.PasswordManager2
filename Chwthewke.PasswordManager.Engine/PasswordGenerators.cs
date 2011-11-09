using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Chwthewke.PasswordManager.Engine
{
    [Obsolete]
    internal static class PasswordGenerators
    {
        public static IPasswordGenerator AlphaNumeric
        {
            get { return _alphaNumeric; }
        }

        public static IPasswordGenerator Full
        {
            get { return _full; }
        }

        public static IEnumerable<IPasswordGenerator> All
        {
            get { return new ReadOnlyCollection<IPasswordGenerator>( new List<IPasswordGenerator> { _alphaNumeric, _full } ); }
        }


        private static PasswordGenerator Sha512Generator( Guid id, Alphabet alphabet, int passwordLength )
        {
            return new PasswordGenerator( id,
                                          _sha512Factory,
                                          new BaseConverter( alphabet.Length ),
                                          alphabet,
                                          passwordLength );
        }

        private static readonly IHashFactory _sha512Factory = new Sha512Factory( );

        private static readonly PasswordGenerator _alphaNumeric =
            Sha512Generator( Guid.Parse( "{74728a10-33d4-4245-b7c9-5d72fc424c41}" ), Alphabets.Symbols62, 12 );

        private static readonly PasswordGenerator _full =
            Sha512Generator( Guid.Parse( "{ccf1451c-4b30-45a4-99b0-d54ec3c3a7ee}" ), Alphabets.Symbols92, 10 );
    }
}