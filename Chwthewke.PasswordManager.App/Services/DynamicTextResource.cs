using System;
using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class DynamicTextResource : ITextResource
    {
        public DynamicTextResource( )
        {
            _delegate = new EmptyTextResource(  );
        }

        private ITextResource _delegate;
        public ITextResource Delegate
        {
            get { return _delegate; }
            set
            {
                if ( value == null )
                    throw new ArgumentNullException( "value" );

                _delegate = value;
            }
        }

        public TextReader OpenReader( )
        {
            return Delegate.OpenReader( );
        }

        public TextWriter OpenWriter( )
        {
            return Delegate.OpenWriter( );
        }
    }

    public class EmptyTextResource : ITextResource
    {
        public TextReader OpenReader( )
        {
            // TODO uh-oh, shouldn't this rather take advantage of the IPasswordData injection seam ?
            return new StringReader( "<password-store version=\"0\" />" );
        }

        public TextWriter OpenWriter( )
        {
            return TextWriter.Null;
        }
    }
}