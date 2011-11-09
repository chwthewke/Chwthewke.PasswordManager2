using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Engine
{
    public interface IPasswordDerivationEngine
    {
        DerivedPassword Derive( PasswordRequest request );

        IEnumerable<Guid> PasswordGenerators { get; }
    }
}