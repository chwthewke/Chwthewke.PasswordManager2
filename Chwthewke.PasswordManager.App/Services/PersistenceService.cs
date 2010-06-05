using System;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PersistenceService : IPersistenceService
    {
        public PersistenceService( Settings settings,
                                   IPasswordStore passwordStore,
                                   IPasswordStoreSerializer passwordStoreSerializer )
        {
            _settings = settings;
            _passwordStore = passwordStore;
            _passwordStoreSerializer = passwordStoreSerializer;
        }

        public void Start( )
        {
            CurrentPersistenceService.Start( );
        }

        public void Save( )
        {
            CurrentPersistenceService.Save( );
        }

        public void Stop( )
        {
            CurrentPersistenceService.Stop( );
        }

        private IPersistenceService CurrentPersistenceService
        {
            get
            {
                if ( _settings.PasswordsAreExternal )
                {
                    try
                    {
                        FileInfo passwordsFile = new FileInfo( _settings.ExternalPasswordDatabase );
                        return new ExternalPasswordDatabase( passwordsFile,
                                                             _passwordStore,
                                                             _passwordStoreSerializer );
                    }
                    catch ( Exception e )
                    {
                        Console.WriteLine( e );
                    }
                }

                return new SettingsPasswordDatabase( _settings,
                                                     _passwordStore,
                                                     _passwordStoreSerializer );
            }
        }

        private readonly Settings _settings;
        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordStoreSerializer _passwordStoreSerializer;
    }
}