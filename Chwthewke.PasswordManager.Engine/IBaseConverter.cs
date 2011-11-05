using System;

namespace Chwthewke.PasswordManager.Engine
{
    [Obsolete]
    internal interface IBaseConverter
    {
        int BytesNeeded( int numDigits );

        byte[ ] ConvertBytesToDigits( byte[ ] bytes, int numDigits );
    }
}