using System;
using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    internal class InMemoryPasswordStore : IPasswordStore
    {
        public string Content { get; set; }

        private void SetContent( string value )
        {
            Content = value;
        }

        public IEnumerable<PasswordDigest> Load( )
        {
            throw new NotImplementedException( );
        }

        public void Save( IEnumerable<PasswordDigest> passwords )
        {
            throw new NotImplementedException( );
        }

        public TextReader OpenReader( )
        {
            return new StringReader( Content );
        }

        public TextWriter OpenWriter( )
        {
            return new FlushingStringWriter( SetContent );
        }
    }
}