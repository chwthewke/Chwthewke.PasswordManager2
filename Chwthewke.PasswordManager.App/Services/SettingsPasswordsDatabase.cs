using System;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class SettingsPasswordsDatabase : IPasswordPersistenceService
    {
        public SettingsPasswordsDatabase( Settings settings,
            IPasswordStore passwordStore, IPasswordStoreSerializer serializer )
        {
            _settings = settings;
            _passwordStore = passwordStore;
            _serializer = serializer;
        }

        public void Start( )
        {
            try
            {
                _serializer.Load( _passwordStore, new StringReader( _settings.PasswordStore ) );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        public void Save( )
        {
            TextWriter writer = new StringWriter( );
            _serializer.Save( _passwordStore, writer );
            _settings.PasswordStore = writer.ToString( );
        }

        public void Stop( )
        {
            Save( );
        }

        private readonly Settings _settings;
        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordStoreSerializer _serializer;
    }
}