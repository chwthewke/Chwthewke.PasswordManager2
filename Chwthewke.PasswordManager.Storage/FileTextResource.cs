using System;
using System.IO;
using System.Runtime.Serialization;
using System.Text;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordsFileException : Exception
    {
        public PasswordsFileException( )
        {
        }

        public PasswordsFileException( string message ) : base( message )
        {
        }

        public PasswordsFileException( string message, Exception innerException ) : base( message, innerException )
        {
        }

        protected PasswordsFileException( SerializationInfo info, StreamingContext context ) : base( info, context )
        {
        }
    }

    public class FileTextResource : ITextResource
    {
        public FileTextResource( FileInfo passwordsFile )
        {
            if ( passwordsFile == null )
                throw new ArgumentNullException( "passwordsFile" );
            _passwordsFile = passwordsFile;
        }

        public TextReader OpenReader( )
        {
            return WrappingFileExceptions( ( ) =>
                                           new StreamReader( _passwordsFile.Open( FileMode.OpenOrCreate, FileAccess.Read ), new UTF8Encoding( false ) ) );
        }

        public TextWriter OpenWriter( )
        {
            return WrappingFileExceptions( ( ) =>
                                           new StreamWriter( _passwordsFile.Open( FileMode.Create, FileAccess.Write ), new UTF8Encoding( false ) ) );
        }


        private T WrappingFileExceptions<T>( Func<T> function )
        {
            try
            {
                return function( );
            }
            catch ( ArgumentException e )
            {
                throw passwordsFileException( e );
            }
            catch ( UnauthorizedAccessException e )
            {
                throw passwordsFileException( e );
            }
            catch ( DirectoryNotFoundException e )
            {
                throw passwordsFileException( e );
            }
            catch ( IOException e )
            {
                throw passwordsFileException( e );
            }
        }

        private Exception passwordsFileException( Exception innerException )
        {
            throw new PasswordsFileException( "A file error occured", innerException );
        }

        private readonly FileInfo _passwordsFile;
    }
}