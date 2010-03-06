using System;
using System.Security;

namespace Chwthewke.PasswordManager.Engine
{
    public interface IPasswordGenerator
    {
        Guid Id { get; }

        string MakePassword( string key, SecureString masterPassword );
    }
}