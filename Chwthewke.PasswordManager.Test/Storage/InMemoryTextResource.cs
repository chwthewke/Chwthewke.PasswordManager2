using System.IO;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    internal class InMemoryTextResource : ITextResource
    {
        public string Content { get; set; }

        public InMemoryTextResource( )
        {
            Content = string.Empty;
        }

        public TextReader OpenReader( )
        {
            return new StringReader( Content );
        }

        public TextWriter OpenWriter( )
        {
            return new FlushingStringWriter( v => Content = v );
        }
    }
}