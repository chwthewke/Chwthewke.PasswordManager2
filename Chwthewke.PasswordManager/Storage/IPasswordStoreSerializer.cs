using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStoreSerializer
    {
        void Save( IPasswordStore passwordStore, TextWriter writer );
        void Load( IPasswordStore passwordStore, TextReader inputStream );
    }
}