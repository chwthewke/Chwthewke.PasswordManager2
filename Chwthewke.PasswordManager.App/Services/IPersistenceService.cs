namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPersistenceService
    {
        void Start( );
        void Save( );
        void Stop( );
    }
}