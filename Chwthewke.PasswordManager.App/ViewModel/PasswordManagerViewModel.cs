using Chwthewke.PasswordManager.App.Services;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordManagerViewModel
    {
        public PasswordManagerViewModel( PasswordListViewModel passwordList, IPersistenceService persistenceService )
        {
            _passwordList = passwordList;
            _persistenceService = persistenceService;
        }

        public PasswordListViewModel PasswordList
        {
            get { return _passwordList; }
        }

        private readonly PasswordListViewModel _passwordList;
        private readonly IPersistenceService _persistenceService;
    }
}