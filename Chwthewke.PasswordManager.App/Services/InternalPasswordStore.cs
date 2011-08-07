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
            using ( TextReader reader = OpenReader( ) )
                return _serializer.Load( reader );
        }

        public TextReader OpenReader( )
        {
            return new StringReader( _settings.PasswordDatabase );
        }

        public void Save( IEnumerable<PasswordDigest> passwords )
        {
            using ( TextWriter writer = OpenWriter( ) )
                _serializer.Save( passwords, writer );
        }

        public TextWriter OpenWriter( )
        {
            return new SettingsPasswordDatabaseWriter( _settings );
        }

        private readonly Settings _settings;
        private readonly IPasswordSerializer _serializer;
    }

    internal class SettingsPasswordDatabaseWriter : StringWriter
    {
        public SettingsPasswordDatabaseWriter( Settings settings )
        {
            _settings = settings;
        }

        public override void Flush( )
        {
            _settings.PasswordDatabase = ToString( );
            _settings.Save( );
        }

        private readonly Settings _settings;
    }
}