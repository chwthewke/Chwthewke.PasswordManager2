using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class InternalPasswordStore : IPasswordStore
    {
        public InternalPasswordStore( IPasswordSerializer serializer, Settings settings )
        {
            _settings = settings;
            _serializer = serializer;
        }

        public IEnumerable<PasswordDigest> Load( )
        {
            return _serializer.Load( new StringReader( _settings.PasswordDatabase ) );
        }

        public void Save( IEnumerable<PasswordDigest> passwords )
        {
            TextWriter writer = new StringWriter( );
            _serializer.Save( passwords, writer );
            _settings.PasswordDatabase = writer.ToString( );

            _settings.Save( );
        }

        private readonly Settings _settings;
        private readonly IPasswordSerializer _serializer;
    }
}