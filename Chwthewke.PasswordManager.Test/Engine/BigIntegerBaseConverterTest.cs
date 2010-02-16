using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class BigIntegerBaseConverterTest : BaseConverterTestBase
    {
        protected override IBaseConverter GetConverter( int theBase )
        {
            return new BigIntegerBaseConverter( theBase );
        }
    }
}