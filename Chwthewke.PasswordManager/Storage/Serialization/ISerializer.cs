using System.IO;

namespace Chwthewke.PasswordManager.Storage.Serialization
{
    public interface ISerializer
    {
        void Save( TextWriter writer );

        void Load( TextReader reader );
    }
}