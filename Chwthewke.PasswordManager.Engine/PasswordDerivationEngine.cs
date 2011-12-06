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

        public IEnumerable<Guid> PasswordGeneratorIds
        {
            get { return _generators.Keys; }
        }

        public IEnumerable<Guid> LegacyPasswordGeneratorIds
        {
            get { return new[ ] { PasswordGenerators.LegacyAlphaNumeric, PasswordGenerators.LegacyFull }; }
        }

        private readonly IDictionary<Guid, PasswordGenerator> _generators;
    }
}