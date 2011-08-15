using System;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class PasswordStoreProvider
    {
        public PasswordStoreProvider( Settings settings, IPasswordSerializer passwordSerializer )
        {
            _settings = settings;
            _passwordSerializer = passwordSerializer;
        }

        public IPasswordStore GetPasswordStore( )
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

        private readonly Settings _settings;
        private readonly IPasswordSerializer _passwordSerializer;
    }
}
