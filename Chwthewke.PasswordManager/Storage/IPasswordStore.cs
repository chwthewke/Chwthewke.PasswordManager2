using System;
using System.Collections.Generic;
using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordStore {
        [Obsolete]
        IEnumerable<PasswordDigest> Load( );
        [Obsolete]
        void Save( IEnumerable<PasswordDigest> passwords );
        TextReader OpenReader( );
        TextWriter OpenWriter( );
    }
}