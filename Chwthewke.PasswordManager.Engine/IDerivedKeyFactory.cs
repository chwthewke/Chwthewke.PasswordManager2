namespace Chwthewke.PasswordManager.Engine
{
    internal interface IDerivedKeyFactory
    {
        byte[ ] DeriveKey( byte[ ] salt, byte[ ] password, int iterations, int byteCount );


    }
}