using System;
using System.IO;
using System.Text;

namespace Chwthewke.PasswordManager.Storage
{
    public class FilePasswordStore : IPasswordStore
    {
        public FilePasswordStore( FileInfo passwordsFile )
        {
            if ( passwordsFile == null )
                throw new ArgumentNullException( "passwordsFile" );
            _passwordsFile = passwordsFile;
        }


        public TextReader OpenReader( )
        {
            return new StreamReader( _passwordsFile.Open( FileMode.OpenOrCreate, FileAccess.Read ), new UTF8Encoding( false ) );
        }

        public TextWriter OpenWriter( )
        {
            return new StreamWriter( _passwordsFile.Open( FileMode.Create, FileAccess.Write ), new UTF8Encoding( false ) );
        }

        private readonly FileInfo _passwordsFile;
    }
}