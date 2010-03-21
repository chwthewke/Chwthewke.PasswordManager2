using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    public interface IHash2
    {
        IHash2 Append( byte[ ] bytes );

        IHash2 Append( string str, Encoding encoding );

        IHash2 Append( SecureString secureString, Encoding encoding );

        byte[ ] GetValue( );
    }
}