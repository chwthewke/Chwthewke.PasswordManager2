using System;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class SettingsPasswordDatabase : IPersistenceService
    {
        public SettingsPasswordDatabase( Settings settings,
                                          IPasswordRepository passwordRepository,
                                          IPasswordSerializer serializer )
        {
            _settings = settings;
            _passwordRepository = passwordRepository;
            _serializer = serializer;
        }

        public void Start( )
        {
            try
            {
                _serializer.Load( _passwordRepository, new StringReader( _settings.PasswordDatabase ) );
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        public void Save( )
        {
            TextWriter writer = new StringWriter( );
            _serializer.Save( _passwordRepository, writer );
            _settings.PasswordDatabase = writer.ToString( );
        }

        public void Stop( )
        {
            Save( );
            _settings.Save( );
        }

        private readonly Settings _settings;
        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordSerializer _serializer;
    }
}