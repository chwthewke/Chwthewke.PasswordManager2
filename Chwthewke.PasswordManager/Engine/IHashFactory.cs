namespace Chwthewke.PasswordManager.Engine
{
    public interface IHashFactory
    {
        IHash2 GetHash( );
        int HashSize { get; }
    }
}