using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Test.Engine
{
    public static class Util
    {
        internal static SecureString Secure( IEnumerable<char> s )
        {
            SecureString result = new SecureString( );
            foreach ( char c in s )
                result.AppendChar( c );
            return result;
        }
    }
}