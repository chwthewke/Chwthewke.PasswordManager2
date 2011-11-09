using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    [Obsolete]
    public interface IPasswordSerializer
    {
        void Save( IEnumerable<PasswordDigest> passwordDigests, ITextResource store );


        IEnumerable<PasswordDigest> Load( ITextResource store );
    }
}