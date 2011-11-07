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

        /// <summary>
        /// Convenience method equivalent to <code>Update( password, password.Delete( deletedOn ) )</code>
        /// </summary>
        /// <param name="password"></param>
        /// <param name="deletedOn"></param>
        /// <returns></returns>
        bool DeletePassword( PasswordDigestDocument password, DateTime deletedOn );
    }
}
