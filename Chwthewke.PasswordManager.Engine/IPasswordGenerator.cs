using System;
using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Engine
{
    [Obsolete]
    public interface IPasswordGenerator
    {
        Guid Id { get; }

        string MakePassword( string key, SecureString masterPassword );

        IEnumerable<string> MakePasswords( string key, SecureString masterPassword );
    }
}