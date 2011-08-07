using System;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PersistenceService : IPersistenceService
    {
        public PersistenceService( Func<IPasswordStore> passwordStoreProvider,
                                   IPasswordRepository passwordRepository )
        {
            _passwordStoreProvider = passwordStoreProvider;
            _passwordRepository = passwordRepository;
        }

        public void Init( )
        {
            foreach ( PasswordDigest passwordDigest in _passwordStoreProvider(  ).Load( ) )
                _passwordRepository.AddOrUpdate( passwordDigest );
        }

        public void Save( )
        {
            // TODO implement load-merge function
            // add Lock( ) API to IPasswordStore ?
            _passwordStoreProvider( ).Save( _passwordRepository.Passwords );
        }

        private readonly Func<IPasswordStore> _passwordStoreProvider;
        private readonly IPasswordRepository _passwordRepository;
    }
}