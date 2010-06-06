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

        public void Init( )
        {
            foreach ( PasswordDigest passwordDigest in CurrentPasswordStore.Load( ) )
                _passwordRepository.AddOrUpdate( passwordDigest );
        }

        public void Save( )
        {
            CurrentPasswordStore.Save( _passwordRepository.Passwords );
        }

        private IPasswordStore CurrentPasswordStore
        {
            get
            {
                if ( _settings.PasswordsAreExternal )
                {
                    try
                    {
                        FileInfo passwordsFile = new FileInfo( _settings.ExternalPasswordDatabase );
                        return new ExternalPasswordStore( _passwordSerializer, passwordsFile );
                    }
                    catch ( Exception e )
                    {
                        Console.WriteLine( e );
                    }
                }

                return new InternalPasswordStore( _passwordSerializer, _settings );
            }
        }

        private readonly Settings _settings;
        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordSerializer _passwordSerializer;
    }
}