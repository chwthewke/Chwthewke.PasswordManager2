using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    class PasswordGeneratorNames
    {
        public static string GeneratorName( IPasswordGenerator generator )
        {
            return GeneratorNames[ generator.Id ];
        }

        public static string GeneratorName( Guid generatorId )
        {
            return GeneratorNames[ generatorId ];
        }

        private static readonly IDictionary<Guid, string> GeneratorNames =
            new Dictionary<Guid, string>( )
                {
                    { PasswordGenerators.AlphaNumeric.Id, "Alphanumeric" },
                    { PasswordGenerators.Full.Id, "Complex"}
                };
    }
}
