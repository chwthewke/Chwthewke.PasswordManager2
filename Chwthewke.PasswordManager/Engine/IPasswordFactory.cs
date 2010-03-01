using System;
using System.Security;

namespace Chwthewke.PasswordManager.Engine
{
    public interface IPasswordFactory
    {
        Guid Id { get; }

        string MakePassword( string key, SecureString masterPassword );
    }
}