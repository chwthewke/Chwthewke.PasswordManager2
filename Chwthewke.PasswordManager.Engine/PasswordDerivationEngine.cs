using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Engine
{
    internal class PasswordDerivationEngine : IPasswordDerivationEngine
    {
        public PasswordDerivationEngine( IDictionary<Guid, PasswordGenerator> generators )
        {
            _generators = generators;
        }

        public IDerivedPassword Derive( PasswordRequest request )
        {
            return _generators[ request.PasswordGenerator ].Derive( request );
        }

        public IEnumerable<Guid> PasswordGenerators
        {
            get { return _generators.Keys; }
        }

        private readonly IDictionary<Guid, PasswordGenerator> _generators;
    }
}