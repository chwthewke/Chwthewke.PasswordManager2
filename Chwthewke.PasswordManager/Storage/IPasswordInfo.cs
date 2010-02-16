using System;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordInfo
    {
        string Key { get; }
        
        byte[ ] Hash { get; }

        Guid MasterPasswordId { get; }

        DateTime CreationTime { get; }
    }
}