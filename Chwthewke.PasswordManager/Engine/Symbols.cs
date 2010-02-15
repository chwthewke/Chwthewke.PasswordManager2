using System;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal class Symbols
    {
        private readonly string _symbols;

        public Symbols( string symbols )
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

        internal int Length { get { return _symbols.Length; } }


        internal static readonly Symbols Symbols92 = new Symbols( SymbolsString92 );
        internal static readonly Symbols Symbols62 = new Symbols( SymbolsString62 );

        internal const string SymbolsString92 =
            @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&""'(-_)=#{[|\@]}$%*<>,?;.:/!^`";

        internal const string SymbolsString62 =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    }
}