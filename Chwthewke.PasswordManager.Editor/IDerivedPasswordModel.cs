using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IDerivedPasswordModel
    {
        Guid Generator { get; }

        IDerivedPassword DerivedPassword { get; }
        void UpdateDerivedPassword( string key, SecureString masterPassword, int iteration );
    }
}