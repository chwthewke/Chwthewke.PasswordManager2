namespace Chwthewke.PasswordManager.Engine
{
    public static class PasswordManagerEngine
    {
        public static IPasswordDerivationEngine DerivationEngine
        {
            get { return new PasswordDerivationEngine( PasswordGenerators.Generators ); }
        }
    }
}
