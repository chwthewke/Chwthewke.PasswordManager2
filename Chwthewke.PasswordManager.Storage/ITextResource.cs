using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface ITextResource
    {
        TextReader OpenReader( );
        TextWriter OpenWriter( );
    }
}