using System.Collections.Generic;
using System.IO;
using System.Text;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class ExternalPasswordStore : IPasswordStore
    {
        public ExternalPasswordStore( IPasswordSerializer serializer, FileInfo passwordsFile )
        {
            _serializer = serializer;
            _passwordsFile = passwordsFile;
        }

        public IEnumerable<PasswordDigest> Load( )
        {
            using ( TextReader reader = new StreamReader( _passwordsFile.Open( FileMode.OpenOrCreate, FileAccess.Read ), new UTF8Encoding( false ) ) )
            {
                return _serializer.Load( reader );
            }
        }

        public void Save( IEnumerable<PasswordDigest> passwords )
        {
            using ( TextWriter writer = new StreamWriter( _passwordsFile.Open( FileMode.Create, FileAccess.Write ), new UTF8Encoding( false ) ) )
            {
                _serializer.Save( passwords, writer );
            }
        }

        private readonly IPasswordSerializer _serializer;
        private readonly FileInfo _passwordsFile;

    }
}