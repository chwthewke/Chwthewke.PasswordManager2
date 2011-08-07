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
            using ( TextReader reader = OpenReader( ) )
            {
                return _serializer.Load( reader );
            }
        }

        public TextReader OpenReader( )
        {
            return new StreamReader( _passwordsFile.Open( FileMode.OpenOrCreate, FileAccess.Read ), new UTF8Encoding( false ) );
        }

        public void Save( IEnumerable<PasswordDigest> passwords )
        {
            using ( TextWriter writer = OpenWriter( ) )
            {
                _serializer.Save( passwords, writer );
            }
        }

        public TextWriter OpenWriter( )
        {
            return new StreamWriter( _passwordsFile.Open( FileMode.Create, FileAccess.Write ), new UTF8Encoding( false ) );
        }


        private readonly IPasswordSerializer _serializer;
        private readonly FileInfo _passwordsFile;

    }
}