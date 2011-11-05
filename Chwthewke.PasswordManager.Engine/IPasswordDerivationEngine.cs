namespace Chwthewke.PasswordManager.Engine
{
    public interface IPasswordDerivationEngine
    {
        DerivedPassword Derive(PasswordRequest request);
    }
}