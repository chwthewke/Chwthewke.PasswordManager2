using System;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PersistenceService : IPersistenceService
    {
        public PersistenceService( Settings settings,
                                   IPasswordRepository passwordRepository,
                                   IPasswordSerializer passwordSerializer )
        {
            _settings = settings;
            _passwordRepository = passwordRepository;
            _passwordSerializer = passwordSerializer;
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
                                                             _passwordRepository,
                                                             _passwordSerializer );
                    }
                    catch ( Exception e )
                    {
                        Console.WriteLine( e );
                    }
                }

                return new SettingsPasswordDatabase( _settings,
                                                     _passwordRepository,
                                                     _passwordSerializer );
            }
        }

        private readonly Settings _settings;
        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordSerializer _passwordSerializer;
    }
}