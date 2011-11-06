using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    [Obsolete]
    public interface IPasswordSerializer
    {
        void Save( IEnumerable<PasswordDigest> passwordDigests, IPasswordStore store );


        IEnumerable<PasswordDigest> Load( IPasswordStore store );
    }
}