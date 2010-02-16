namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordTester
    {
        bool TestPassword( IPasswordInfo passwordInfo, byte[ ] masterPassword );
    }
}