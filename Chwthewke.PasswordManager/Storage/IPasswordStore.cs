using System;
using System.Collections.Generic;
using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore {
        TextReader OpenReader( );
        TextWriter OpenWriter( );
    }
}