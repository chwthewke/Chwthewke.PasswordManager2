using System;
using System.IO;
using System.Text;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class ExternalPasswordDatabase : IPersistenceService
    {
        public ExternalPasswordDatabase( FileInfo passwordsFile, IPasswordStore passwordStore, IPasswordStoreSerializer serializer )
        {
            _passwordStore = passwordStore;
            _serializer = serializer;
            _passwordsFile = passwordsFile;
        }

        public void Start( )
        {
            try
            {
                using ( FileStream stream = _passwordsFile.Open( FileMode.OpenOrCreate, FileAccess.Read ) )
                using ( TextReader reader = new StreamReader( stream, new UTF8Encoding( false ) ) )
                {
                    _serializer.Load( _passwordStore, reader );
                }
            }
            catch ( Exception e )
            {
                Console.WriteLine( e );
            }
        }

        public void Save( )
        {
            using ( FileStream stream = _passwordsFile.Open( FileMode.Create, FileAccess.Write ) )
            using ( TextWriter writer = new StreamWriter( stream, new UTF8Encoding( false ) ) )
            {
                _serializer.Save( _passwordStore, writer );
            }
        }

        public void Stop( )
        {
            Save( );
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordStoreSerializer _serializer;
        private readonly FileInfo _passwordsFile;
    }
}