namespace Chwthewke.PasswordManager.Storage
{
    internal interface IPasswordRepository
    {
        IPassword this[ string key ] { get; }
    }
}