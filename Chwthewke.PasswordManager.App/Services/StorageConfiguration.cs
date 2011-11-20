using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class StorageConfiguration : IStorageConfiguration
    {
        public StorageConfiguration( Settings settings, IPasswordManagerStorage storage )
        {
            _settings = settings;
            _storage = storage;
            ApplyStorageTypeSetting( );
        }

        public void SelectExternalStorage( FileInfo externalFile )
        {
            ImportOldPasswords( s =>
                                    {
                                        s.PasswordsAreExternal = true;
                                        s.ExternalPasswordDatabaseFile = externalFile.FullName;
                                        s.SavedPasswordData = string.Empty;
                                        s.Save( );
                                    } );
        }

        public void SelectInternalStorage(  )
        {
            ImportOldPasswords( s =>
                                    {
                                        s.PasswordsAreExternal = false;
                                        s.ExternalPasswordDatabaseFile = string.Empty;
                                    } );
        }

        private void ImportOldPasswords( Action<Settings> configureSettings )
        {
            var currentPasswords = LoadCurrentPasswords( );

            var oldSettings = SettingsData.CopyOf( _settings );

            try
            {
                configureSettings( _settings );

                ApplyStorageTypeSetting( );
                _storage.PasswordRepository.Merge( currentPasswords );

            }
            catch ( PasswordsFileException )
            {
                oldSettings.Apply( _settings );
                throw;
            }
        }

        private IEnumerable<PasswordDigestDocument> LoadCurrentPasswords( )
        {
            try
            {
                return _storage.PasswordRepository.PasswordData.LoadPasswords( );
            }
            catch ( PasswordsFileException )
            {
                return Enumerable.Empty<PasswordDigestDocument>( );
            }
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
            return XmlPasswordData.From( new FileTextResource( new FileInfo( _settings.ExternalPasswordDatabaseFile ) ) );
        }


        private readonly Settings _settings;
        private readonly IPasswordManagerStorage _storage;
    }

    public enum StorageType
    {
        Internal,
        External
    }

    internal class SettingsData
    {
        internal static SettingsData CopyOf( Settings settings )
        {
            return new SettingsData
                       {
                           PasswordsAreExternal = settings.PasswordsAreExternal,
                           ExternalPasswordDatabaseFile = settings.ExternalPasswordDatabaseFile,
                           SavedPasswordData = settings.SavedPasswordData
                       };
        }

        internal void Apply( Settings settings )
        {
            settings.PasswordsAreExternal = PasswordsAreExternal;
            settings.ExternalPasswordDatabaseFile = ExternalPasswordDatabaseFile;
            settings.SavedPasswordData = SavedPasswordData;
            settings.Save( );
        }

        public bool PasswordsAreExternal { get; private set; }
        public string ExternalPasswordDatabaseFile { get; private set; }
        public string SavedPasswordData { get; private set; }
    }
}