using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    // TODO this is still in use, but is this still useful ?
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