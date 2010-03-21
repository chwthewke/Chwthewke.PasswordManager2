namespace Chwthewke.PasswordManager.Engine
{
    public interface IHashFactory
    {
        IHash GetHash( );
        int HashSize { get; }
    }
}