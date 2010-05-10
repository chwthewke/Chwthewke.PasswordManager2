namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPasswordPersistenceService
    {
        void Start( );
        void Save( );
        void Stop( );
    }
}