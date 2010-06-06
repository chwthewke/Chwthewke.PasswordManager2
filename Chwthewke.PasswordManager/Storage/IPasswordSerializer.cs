using System.Collections.Generic;
using System.IO;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordSerializer
    {
        void Save( IPasswordRepository passwordRepository, TextWriter writer );

        void Save( IEnumerable<PasswordDigest> passwordDigests, TextWriter writer );
        IEnumerable<PasswordDigest> Load( TextReader textReader );
    }
}