using System.IO;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IStorageConfiguration
    {
        void SelectExternalStorage( FileInfo externalFile );
        void SelectInternalStorage(  );
        StorageType StorageType { get; }
    }
}