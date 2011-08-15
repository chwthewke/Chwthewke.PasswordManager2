using System;

namespace Chwthewke.PasswordManager.Storage
{
    [Obsolete]
    public interface IPersistenceService
    {
        void Init( );
        void Save( );
    }
}