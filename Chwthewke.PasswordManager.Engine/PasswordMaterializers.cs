namespace Chwthewke.PasswordManager.Engine
{
    internal static class PasswordMaterializers
    {
        internal static readonly PasswordMaterializer Full = new PasswordMaterializer(SymbolsString92);
        internal static readonly PasswordMaterializer AlphaNumeric = new PasswordMaterializer(SymbolsString62);

        internal static readonly PasswordMaterializer Full2017 = new PasswordMaterializer(SymbolsString92, 90);
        internal static readonly PasswordMaterializer AlphaNumeric2017 = new PasswordMaterializer(SymbolsString62, 90);

        internal const string SymbolsString92 =
            @"ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789&""'(-_)=#{[|\@]}$%*<>,?;.:/!^`";

        internal const string SymbolsString62 =
            "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789";
    }
}