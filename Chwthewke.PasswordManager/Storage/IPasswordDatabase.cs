using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordDatabase
    {
        IPasswordStore Source { get; set; }
        IList<PasswordDigest> Passwords { get; }
        PasswordDigest FindByKey(string key);
        void Reload( );
        void AddOrUpdate( PasswordDigest password );
        void Remove( string key );
    }
}