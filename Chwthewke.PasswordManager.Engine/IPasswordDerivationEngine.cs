using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Engine
{
    public interface IPasswordDerivationEngine
    {
        IDerivedPassword Derive( PasswordRequest request );

        IEnumerable<Guid> PasswordGeneratorIds { get; }

        IEnumerable<Guid> LegacyPasswordGeneratorIds { get; }
    }
}