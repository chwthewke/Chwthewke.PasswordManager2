using System;
using System.Linq;
using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal class PasswordFactory : IPasswordFactory
    {
        public const string Salt = "tsU&yUaZulAs4eOV";

        private readonly IBaseConverter _converter;
        private readonly Symbols _symbols;
        private readonly int _length;
        private const int MinLength = 8;

        public PasswordFactory( IBaseConverter converter, Symbols symbols, int length )
        {
            if ( converter == null )
                throw new ArgumentNullException( "converter" );
            if ( symbols == null )
                throw new ArgumentNullException( "symbols" );
            if ( converter.Base != symbols.Length )
                throw new ArgumentException( "The converter's base must match the symbols length" );
            if ( converter.UsedBytes( length ) > Sha512.Size )
                throw new ArgumentException( "Requested password length too bug", "length" );
            if ( length < MinLength )
                throw new ArgumentException( "Requested password length too small, must be at least" + MinLength,
                                             "length" );
            _converter = converter;
            _length = length;
            _symbols = symbols;
        }

        public string MakePassword( string key, SecureString masterPassword )
        {
            byte[ ] hash = HashTogetherWithSalt( key, masterPassword );
            byte[ ] passwordBytes = _converter.Convert( hash, _length );
            return _symbols.ToString( passwordBytes );
        }

        private static byte[ ] HashTogetherWithSalt( string key, SecureString masterPassword )
        {
            byte[ ] saltBytes = Encoding.UTF8.GetBytes( Salt );
            byte[ ] keyBytes = Encoding.UTF8.GetBytes( key );
            byte[ ] masterPasswordBytes = null;
            byte[ ] src = null;
            try
            {
                masterPasswordBytes = Secure.GetBytes( Encoding.UTF8, masterPassword );
                src = ConcatArrays( saltBytes, masterPasswordBytes, keyBytes );
                return Sha512.Hash( src );
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
            for ( int i = 0, accLength = 0 ; i < arrays.Length ; accLength += arrays[ i++ ].Length )
                Array.Copy( arrays[ i ], 0, result, accLength, arrays[ i ].Length );
            return result;
        }

        private static void ZeroArray( Array array )
        {
            if ( array == null ) return;
            Array.Clear( array, 0, array.Length );
        }
    }
}