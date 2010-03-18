using System;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordDigester {
        PasswordDigest Digest( string key,
                                               string generatedPassword,
                                               Guid masterPasswordId,
                                               Guid passwordGeneratorId,
                                               string note );
    }
}