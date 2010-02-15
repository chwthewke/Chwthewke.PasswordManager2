using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    public class ArrayBaseConverterTest : BaseConverterTestBase
    {
        internal override IBaseConverter GetConverter( int theBase )
        {
            return new ArrayBaseConverter( theBase );
        }
    }
}