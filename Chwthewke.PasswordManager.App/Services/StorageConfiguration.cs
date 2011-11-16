using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class StorageConfiguration
    {
        public StorageConfiguration( Settings settings, IPasswordManagerStorage storage )
        {
            _settings = settings;
            _storage = storage;
            ApplyStorageTypeSetting( );
        }

        public void SelectExternalStorage( FileInfo externalFile )
        {
            
        }

        public void SelectInternalStorage(  )
        {
            var currentPasswords = _storage.PasswordRepository.PasswordData.LoadPasswords(  );
            _settings.PasswordsAreExternal = false;
            _settings.PasswordDatabase = string.Empty;
            ApplyStorageTypeSetting( );
            _storage.PasswordRepository.Merge( currentPasswords );
        }

        public StorageType StorageType { get; private set; }

        private void ApplyStorageTypeSetting( )
        {
            if ( _settings.PasswordsAreExternal )
            {
                StorageType = StorageType.External;
                _storage.PasswordRepository.PasswordData = ExternalPasswordData( );
            }
            else
            {
                StorageType= StorageType.Internal;
                _storage.PasswordRepository.PasswordData = InternalPasswordData( );
            }
        }

        private IPasswordData InternalPasswordData( )
        {
            return XmlPasswordData.From( new SettingsTextResource( _settings ) );
        }

        private IPasswordData ExternalPasswordData( )
        {
            return XmlPasswordData.From( new FileTextResource( new FileInfo( _settings.ExternalPasswordDatabase ) ) );
        }


        private readonly Settings _settings;
        private readonly IPasswordManagerStorage _storage;
    }

    public enum StorageType
    {
        Internal,
        External
    }
}