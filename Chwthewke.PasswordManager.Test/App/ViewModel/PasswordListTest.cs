using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordListTest
    {
        [ SetUp ]
        public void SetUpPasswordList( )
        {
            _passwordList = new PasswordListViewModel( null, null );
        }

        private PasswordListViewModel _passwordList;
    }
}