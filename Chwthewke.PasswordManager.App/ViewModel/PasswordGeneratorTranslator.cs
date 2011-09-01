﻿using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    internal class PasswordGeneratorTranslator
    {
        public static string NameKey( IPasswordGenerator generator )
        {
            return NameKey( generator.Id );
        }

        public static string NameKey( Guid generatorId )
        {
            return "PasswordGenerator" + generatorId.ToString( "N" );
        }

        public static string DescriptionKey( IPasswordGenerator generator )
        {
            return DescriptionKey( generator.Id );
        }

        public static string DescriptionKey( Guid generatorId )
        {
            return "PasswordGeneratorDescription" + generatorId.ToString( "N" );
        }

    }
}