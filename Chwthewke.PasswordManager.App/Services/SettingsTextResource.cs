using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class SettingsTextResource : ITextResource
    {
        public SettingsTextResource( Settings settings )
        {
            _settings = settings;
        }


        public TextReader OpenReader( )
        {
            return new StringReader( _settings.PasswordDatabase );
        }

        public TextWriter OpenWriter( )
        {
            return new FlushingStringWriter( SaveDatabase );
        }

        private void SaveDatabase( string s )
        {
            _settings.PasswordDatabase = s;
            _settings.Save( );
        }


        private readonly Settings _settings;
    }
}