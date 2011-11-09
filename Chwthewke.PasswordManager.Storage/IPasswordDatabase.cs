using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    [Obsolete]
    public interface IPasswordDatabase
    {
        ITextResource Source { get; set; }
        IList<PasswordDigest> Passwords { get; }
        PasswordDigest FindByKey( string key );
        void Reload( );
        void AddOrUpdate( PasswordDigest password );
        void Remove( string key );
    }
}