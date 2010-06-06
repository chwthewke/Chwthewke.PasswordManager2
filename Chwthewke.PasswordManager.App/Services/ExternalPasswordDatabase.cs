using System;
using System.IO;
using System.Text;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class ExternalPasswordDatabase : IPersistenceService
    {
        public ExternalPasswordDatabase( FileInfo passwordsFile, IPasswordRepository passwordRepository, IPasswordSerializer serializer )
        {
            _passwordRepository = passwordRepository;
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
                    _serializer.Load( _passwordRepository, reader );
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
                _serializer.Save( _passwordRepository, writer );
            }
        }

        public void Stop( )
        {
            Save( );
        }

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordSerializer _serializer;
        private readonly FileInfo _passwordsFile;
    }
}