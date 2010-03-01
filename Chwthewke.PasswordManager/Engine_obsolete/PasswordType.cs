using System;

namespace Chwthewke.PasswordManager.Engine
{
    public class PasswordType
    {
        internal readonly Alphabet _alphabet;
        internal readonly int _length;

        private PasswordType( Alphabet alphabet, int length )
        {
            _alphabet = alphabet;
            _length = length;
        }

        public IPasswordFactory Factory
        {
            get
            {
                return new PasswordFactory( default( Guid ), new Sha512( ),
                                            new BigIntegerBaseConverter( _alphabet.Length ), _alphabet,
                                            _length );
            }
        }

        public static readonly PasswordType Ascii = new PasswordType( Alphabets.Symbols92, 10 );
        public static readonly PasswordType AlphaNumeric = new PasswordType( Alphabets.Symbols62, 12 );
    }
}