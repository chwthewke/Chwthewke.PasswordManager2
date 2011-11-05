using System;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    /// <summary>
    /// <para>
    /// Encodes a sequence of bytes into a string built from a fixed set of characters.
    /// </para>
    /// <para>
    /// Returns strings of a fixed even length, the smallest possible to have at least 2 ^ 64 possible outputs.
    /// </para>
    /// </summary>
    internal class PasswordMaterializer
    {
        private readonly string _symbols;
        private readonly BaseConverter _converter;
        private readonly int _passwordLength;

        public PasswordMaterializer( string symbols )
        {
            if ( symbols == null )
                throw new ArgumentNullException( "symbols" );
            _symbols = symbols;
            _converter = new BaseConverter( _symbols.Length );
            _passwordLength = ComputePasswordLength( _symbols.Length );
        }

        public string ToString( byte[ ] bytes )
        {
            byte[ ] digits = _converter.ConvertBytesToDigits( bytes, _passwordLength );

            StringBuilder builder = new StringBuilder( digits.Length );
            foreach ( byte b in digits )
            {
                if ( b > _symbols.Length )
                    throw new ArgumentException( "Unexpected internal error." );
                builder.Append( _symbols[ b ] );
            }
            return builder.ToString( );
        }

        public int PasswordLength
        {
            get { return _passwordLength; }
        }

        public int BytesNeeded
        {
            get { return _converter.BytesNeeded( _passwordLength ); }
        }

        private static int ComputePasswordLength( int symbolCount )
        {
            return 2 * (int) Math.Ceiling( 32 / Math.Log( symbolCount, 2 ) );
        }
    }
}