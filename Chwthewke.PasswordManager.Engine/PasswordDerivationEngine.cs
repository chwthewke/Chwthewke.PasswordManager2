namespace Chwthewke.PasswordManager.Engine
{
    internal class PasswordDerivationEngine : IPasswordDerivationEngine
    {
        public DerivedPassword Derive( PasswordRequest request )
        {
            return PasswordGenerators2.GeneratorWithId( request.PasswordGenerator ).Derive( request );
        }
    }
}