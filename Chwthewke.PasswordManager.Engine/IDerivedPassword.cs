namespace Chwthewke.PasswordManager.Engine
{
    public interface IDerivedPassword
    {
        string Password { get; }
        PasswordDigest Digest { get; }
    }
}