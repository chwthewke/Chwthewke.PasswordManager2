using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordSerializer
    {
        void Save( IPasswordRepository passwordRepository, TextWriter writer );
        void Load( IPasswordRepository passwordRepository, TextReader inputStream );
    }
}