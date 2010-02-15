namespace Chwthewke.PasswordManager.Engine
{
    public interface IHash
    {
        int Size { get; }
        byte[ ] Hash( byte[ ] bytes );
        byte[ ] Hash( string str );
    }
}