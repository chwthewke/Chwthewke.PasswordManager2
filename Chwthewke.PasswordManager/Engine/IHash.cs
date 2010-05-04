using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal interface IHash
    {
        IHash Append( byte[ ] bytes );

        IHash Append( string str, Encoding encoding );

        IHash Append( SecureString secureString, Encoding encoding );

        byte[ ] GetValue( );
    }
}