namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordManagerViewModel
    {
        public PasswordManagerViewModel( PasswordListViewModel passwordList )
        {
            _passwordList = passwordList;
        }

        public PasswordListViewModel PasswordList
        {
            get { return _passwordList; }
        }

        private readonly PasswordListViewModel _passwordList;
    }
}