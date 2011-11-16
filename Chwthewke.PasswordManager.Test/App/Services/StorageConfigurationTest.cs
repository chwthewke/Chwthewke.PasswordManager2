using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.Services
{
    [ TestFixture ]
    public class StorageConfigurationTest
    {
        private const string TempFile = "temp_passwords.xml";
        private const string OtherTempFile = "other_temp_passwords.xml";

        private IPasswordManagerStorage _storage;
        private Settings _settings;
        private IStorageConfiguration _storageConfiguration;

        [ SetUp ]
        public void SetUpStorageConfiguration( )
        {
            _storage = PasswordManagerStorage.CreateService( new InMemoryPasswordData( ) );
            _settings = new Settings( );
        }

        // external -> internal
        // internal -> external
        // external -> other external

        // merge into target
        // further passwords are saved to new target

        [ Test ]
        public void InitializeWithNonExternalDatabaseLoadsSettingsTextResource( )
        {
            // Set up
            XmlPasswordData.From( new SettingsTextResource( _settings ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Abcd } );
            _settings.PasswordsAreExternal = false;
            // Exercise
            _storageConfiguration = new StorageConfiguration( _settings, _storage );

            // Verify
            Assert.That( _storage.PasswordRepository.PasswordData.LoadPasswords( ), Is.EquivalentTo( new[ ] { TestPasswords.Abcd } ) );
        }

        [ Test ]
        public void InitializeWithExternalDatabaseLoadsFileTextResource( )
        {
            // Set up
            var passwordsFile = new FileInfo( TempFile );
            XmlPasswordData.From( new FileTextResource( passwordsFile ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Efgh } );

            _settings.ExternalPasswordDatabaseFile = passwordsFile.FullName;
            _settings.PasswordsAreExternal = true;

            // Exercise
            _storageConfiguration = new StorageConfiguration( _settings, _storage );

            // Verify
            Assert.That( _storage.PasswordRepository.PasswordData.LoadPasswords( ), Is.EquivalentTo( new[ ] { TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void ChangeFromInternalToExternalMergesPasswords( )
        {
            // Set up
            XmlPasswordData.From( new SettingsTextResource( _settings ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Abcd } );

            var passwordsFile = new FileInfo( TempFile );
            XmlPasswordData.From( new FileTextResource( passwordsFile ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Efgh } );

            _settings.PasswordsAreExternal = false;
            _storageConfiguration = new StorageConfiguration( _settings, _storage );
            // Exercise
            _storageConfiguration.SelectExternalStorage( passwordsFile );
            // Verify
            Assert.That( _storage.PasswordRepository.PasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void ChangeFromInternalToExternalSavesFurtherPasswordsToExternal( )
        {
            // Set up
            var passwordsFile = new FileInfo( TempFile );
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder { Key = "abc " };

            _settings.PasswordsAreExternal = false;
            _storageConfiguration = new StorageConfiguration( _settings, _storage );
            _storageConfiguration.SelectExternalStorage( passwordsFile );
            // Exercise
            _storage.PasswordRepository.SavePassword( password );
            // Verify
            var passwords = XmlPasswordData
                .From( new FileTextResource( passwordsFile ) )
                .LoadPasswords( );

            Assert.That( passwords,
                         Is.EquivalentTo( new[ ] { password } ) );
        }

        [ Test ]
        public void ChangeFromExternalToInternalMergesPasswords( )
        {
            // Set up
            XmlPasswordData.From( new SettingsTextResource( _settings ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Abcd } );

            var passwordsFile = new FileInfo( TempFile );
            XmlPasswordData.From( new FileTextResource( passwordsFile ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Efgh } );

            _settings.PasswordsAreExternal = true;
            _settings.ExternalPasswordDatabaseFile = passwordsFile.FullName;
            _storageConfiguration = new StorageConfiguration( _settings, _storage );
            // Exercise
            _storageConfiguration.SelectInternalStorage( );
            // Verify
            Assert.That( _storage.PasswordRepository.PasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void ChangeFromExternalToInternalSavesFurtherPasswordsToInternal( )
        {
            // Set up
            var passwordsFile = new FileInfo( TempFile );
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder { Key = "abc " };

            _settings.PasswordsAreExternal = true;
            _settings.ExternalPasswordDatabaseFile = passwordsFile.FullName;
            _storageConfiguration = new StorageConfiguration( _settings, _storage );
            _storageConfiguration.SelectInternalStorage( );
            // Exercise
            _storage.PasswordRepository.SavePassword( password );
            // Verify
            var passwords = XmlPasswordData
                .From( new SettingsTextResource( _settings ) )
                .LoadPasswords( );

            Assert.That( passwords,
                         Is.EquivalentTo( new[ ] { password } ) );
        }

        [ Test ]
        public void ChangeFromExternalToExternalMergesPasswords( )
        {
            // Set up
            var otherPasswordsFile = new FileInfo( OtherTempFile );
            XmlPasswordData.From( new FileTextResource( otherPasswordsFile ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Abcd } );

            var passwordsFile = new FileInfo( TempFile );
            XmlPasswordData.From( new FileTextResource( passwordsFile ) )
                .SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Efgh } );

            _settings.PasswordsAreExternal = true;
            _settings.ExternalPasswordDatabaseFile = passwordsFile.FullName;
            _storageConfiguration = new StorageConfiguration( _settings, _storage );
            // Exercise
            _storageConfiguration.SelectExternalStorage( otherPasswordsFile );
            // Verify
            Assert.That( _storage.PasswordRepository.PasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void ChangeFromExternalToExternalSavesFurtherPasswordsToNewerExternal( )
        {
            // Set up
            var otherPasswordFile = new FileInfo( OtherTempFile );
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder { Key = "abc " };

            _settings.PasswordsAreExternal = true;
            _settings.ExternalPasswordDatabaseFile = new FileInfo( TempFile ).FullName;
            _storageConfiguration = new StorageConfiguration( _settings, _storage );
            _storageConfiguration.SelectExternalStorage( otherPasswordFile );
            // Exercise
            _storage.PasswordRepository.SavePassword( password );
            // Verify
            var passwords = XmlPasswordData
                .From( new FileTextResource( otherPasswordFile ) )
                .LoadPasswords( );

            Assert.That( passwords,
                         Is.EquivalentTo( new[ ] { password } ) );
        }


        [ TearDown ]
        public void RemoveTempFile( )
        {
            new FileInfo( TempFile ).Delete( );
            new FileInfo( OtherTempFile ).Delete( );
            _settings.Reset( );
            _settings.Save( );
        }
    }
}