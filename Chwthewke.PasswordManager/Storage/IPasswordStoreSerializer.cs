using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStoreSerializer {
        void Save( IPasswordStore passwordStore, Stream outputStream );
        void Load( IPasswordStore passwordStore, Stream inputStream );
    }
}