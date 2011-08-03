using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class PersistenceService : IPersistenceService
    {
        public PersistenceService( IPasswordStoreProvider passwordStoreProvider,
                                   IPasswordRepository passwordRepository )
        {
            _passwordStoreProvider = passwordStoreProvider;
            _passwordRepository = passwordRepository;
        }

        public void Init( )
        {
            foreach ( PasswordDigest passwordDigest in _passwordStoreProvider.GetPasswordStore(  ).Load( ) )
                _passwordRepository.AddOrUpdate( passwordDigest );
        }

        public void Save( )
        {
            // TODO implement load-merge function
            // add Lock( ) API to IPasswordStore ?
            _passwordStoreProvider.GetPasswordStore( ).Save( _passwordRepository.Passwords );
        }

        private readonly IPasswordStoreProvider _passwordStoreProvider;
        private readonly IPasswordRepository _passwordRepository;
    }
}