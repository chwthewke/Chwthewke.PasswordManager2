using System;
using System.Linq;
using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal class PasswordFactory : IPasswordFactory
    {
        public const string Salt = "tsU&yUaZulAs4eOV";

        public Guid Id
        {
            get { return _id; }
        }

        public PasswordFactory( Guid id, IHash hash, IBaseConverter converter, Alphabet alphabet, int length )
        {
            if ( hash == null )
                throw new ArgumentNullException( "hash" );
            if ( converter == null )
                throw new ArgumentNullException( "converter" );
            if ( alphabet == null )
                throw new ArgumentNullException( "alphabet" );
            if ( converter.Base != alphabet.Length )
                throw new ArgumentException( "The converter's base must match the alphabet length" );
            if ( converter.BytesNeeded( length ) > hash.Size )
                throw new ArgumentException( "Requested password length too large", "length" );

            _hash = hash;
            _converter = converter;
            _length = length;
            _alphabet = alphabet;
            _id = id;
        }

        public string MakePassword( string key, SecureString masterPassword )
        {
            byte[ ] hash = HashTogetherWithSalt( key, masterPassword );
            byte[ ] passwordBytes = _converter.ConvertBytesToDigits( hash, _length );
            return _alphabet.ToString( passwordBytes );
        }

        private byte[ ] HashTogetherWithSalt( string key, SecureString masterPassword )
        {
            byte[ ] saltBytes = Encoding.UTF8.GetBytes( Salt );
            byte[ ] keyBytes = Encoding.UTF8.GetBytes( key );
            byte[ ] masterPasswordBytes = null;
            byte[ ] src = null;
            try
            {
                masterPasswordBytes = Secure.GetBytes( Encoding.UTF8, masterPassword );
                src = ConcatArrays( saltBytes, masterPasswordBytes, keyBytes );
                return _hash.Hash( src );
            }
            finally
            {
                ZeroArray( masterPasswordBytes );
                ZeroArray( src );
            }
        }

        private static T[ ] ConcatArrays<T>( params T[ ][ ] arrays )
        {
            T[ ] result = new T[arrays.Aggregate( 0, ( x, y ) => x + y.Length )];
            for ( int i = 0, accLength = 0; i < arrays.Length; accLength += arrays[ i++ ].Length )
                Array.Copy( arrays[ i ], 0, result, accLength, arrays[ i ].Length );
            return result;
        }

        private static void ZeroArray( Array array )
        {
            if ( array == null ) return;
            Array.Clear( array, 0, array.Length );
        }

        private readonly IHash _hash;
        private readonly IBaseConverter _converter;
        private readonly Alphabet _alphabet;
        private readonly int _length;
        private readonly Guid _id;
    }
}