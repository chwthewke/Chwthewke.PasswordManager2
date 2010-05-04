namespace Chwthewke.PasswordManager.Engine
{
    internal interface IBaseConverter
    {
        int Base { get; }

        int BytesNeeded( int numDigits );

        byte[ ] ConvertBytesToDigits( byte[ ] bytes, int numDigits );
    }
}