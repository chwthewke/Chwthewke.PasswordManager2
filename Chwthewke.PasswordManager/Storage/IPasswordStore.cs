using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore
    {
        TextReader OpenReader( );
        TextWriter OpenWriter( );
    }
}