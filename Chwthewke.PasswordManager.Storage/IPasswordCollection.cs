using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordCollection
    {
        IList<PasswordDigestDocument> LoadPasswords( );

        PasswordDigestDocument LoadPassword( string key );

        bool UpdatePassword( PasswordDigestDocument original, PasswordDigestDocument password );

        bool SavePassword( PasswordDigestDocument password );

        bool DeletePassword( PasswordDigestDocument password, DateTime deletedOn );
    }
}
