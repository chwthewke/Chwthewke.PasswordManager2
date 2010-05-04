namespace Chwthewke.PasswordManager.Engine
{
    internal interface IHashFactory
    {
        IHash GetHash( );
        int HashSize { get; }
    }
}