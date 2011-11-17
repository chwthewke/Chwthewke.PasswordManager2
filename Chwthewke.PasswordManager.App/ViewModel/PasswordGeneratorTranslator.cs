using System;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    internal static class PasswordGeneratorTranslator
    {
        public static string NameKey( Guid generatorId )
        {
            return "PasswordGenerator" + generatorId.ToString( "N" );
        }

        public static string DescriptionKey( Guid generatorId )
        {
            return "PasswordGeneratorDescription" + generatorId.ToString( "N" );
        }
    }
}