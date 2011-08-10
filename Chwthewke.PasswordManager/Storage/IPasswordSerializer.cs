using System;
using System.Collections.Generic;
using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordSerializer
    {
        [Obsolete]
        void Save( IEnumerable<PasswordDigest> passwordDigests, TextWriter writer );
        
        void Save( IEnumerable<PasswordDigest> passwordDigests, IPasswordStore store );

        [Obsolete]
        IEnumerable<PasswordDigest> Load( TextReader textReader );

        IEnumerable<PasswordDigest> Load( IPasswordStore store );
    }
}