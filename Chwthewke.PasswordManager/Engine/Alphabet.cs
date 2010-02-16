using System;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal class Alphabet
    {
        private readonly string _symbols;

        public Alphabet( string symbols )
        {
            _symbols = symbols;
        }

        public string ToString( byte[ ] bytes )
        {
            StringBuilder builder = new StringBuilder( bytes.Length );
            foreach ( byte b in bytes )
            {
                if ( b > Length )
                    throw new ArgumentException(
                        String.Format( "Invalid byte in stream : {0}, only {1} symbols", b, _symbols.Length ), "bytes" );
                builder.Append( _symbols[ b ] );
            }
            return builder.ToString( );
        }

        public int Length
        {
            get { return _symbols.Length; }
        }
    }
}