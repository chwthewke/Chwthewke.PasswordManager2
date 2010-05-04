namespace Chwthewke.PasswordManager.Engine
{
    internal static class Alphabets
    {
        internal static readonly Alphabet Symbols92 = new Alphabet( SymbolsString92 );
        internal static readonly Alphabet Symbols62 = new Alphabet( SymbolsString62 );

        private const string SymbolsString92 =
            @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&""'(-_)=#{[|\@]}$%*<>,?;.:/!^`";

        private const string SymbolsString62 =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    }
}