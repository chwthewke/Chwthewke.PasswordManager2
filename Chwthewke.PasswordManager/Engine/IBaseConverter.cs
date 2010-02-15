namespace Chwthewke.PasswordManager.Engine
{
    public interface IBaseConverter
    {
        int Base { get; }
        int UsedBytes( int length );
        byte[ ] Convert( byte[ ] src, int length );
    }
}