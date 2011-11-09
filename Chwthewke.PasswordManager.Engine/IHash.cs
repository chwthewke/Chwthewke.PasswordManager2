using System;
using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    [Obsolete]
    internal interface IHash
    {
        IHash Append( byte[ ] bytes );

        IHash Append( string str, Encoding encoding );

        IHash Append( SecureString secureString, Encoding encoding );

        byte[ ] GetValue( );
    }
}