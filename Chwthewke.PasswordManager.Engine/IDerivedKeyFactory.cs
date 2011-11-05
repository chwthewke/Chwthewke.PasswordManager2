namespace Chwthewke.PasswordManager.Engine
{
    public interface IDerivedKeyFactory
    {
        byte[ ] DeriveKey( byte[ ] salt, byte[ ] password, int iterations, int byteCount );


    }
}