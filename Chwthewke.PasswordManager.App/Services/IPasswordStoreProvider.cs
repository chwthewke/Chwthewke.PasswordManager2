namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPasswordStoreProvider
    {
        IPasswordStore GetPasswordStore( );
    }
}