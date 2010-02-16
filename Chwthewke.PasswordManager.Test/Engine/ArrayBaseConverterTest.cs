using System;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture, Obsolete ]
    public class ArrayBaseConverterTest : BaseConverterTestBase
    {
        protected override IBaseConverter GetConverter( int theBase )
        {
            return new ArrayBaseConverter( theBase );
        }
    }
}