using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Test.Engine
{
    public static class Util
    {
        public static SecureString ToSecureString( this IEnumerable<char> charSequence )
        {
            SecureString result = new SecureString( );
            foreach ( char c in charSequence )
                result.AppendChar( c );
            return result;
        }
    }
}