using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    public interface IPasswordManagerStorage
    {
        IMasterPasswordMatcher MasterPasswordMatcher { get; }
        IPasswordRepository PasswordRepository { get; }
    }

    internal class DefaultPasswordManagerStorage : IPasswordManagerStorage
    {
        internal DefaultPasswordManagerStorage( IPasswordData passwordData )
        {
            _passwordRepository = new PasswordRepository( passwordData );
        }

        public IMasterPasswordMatcher MasterPasswordMatcher
        {
            get { return new MasterPasswordMatcher2( PasswordManagerEngine.DerivationEngine, _passwordRepository ); }
        }

        public IPasswordRepository PasswordRepository
        {
            get { return _passwordRepository; }
        }

        private readonly IPasswordRepository _passwordRepository;
    }

    public static class PasswordManagerStorage
    {
        public static IPasswordManagerStorage CreateService( ITextResource passwordsResource )
        {
            return new DefaultPasswordManagerStorage( new XmlPasswordData( new PasswordSerializer2( ), passwordsResource ) );
        }

        public static IPasswordManagerStorage CreateService( IPasswordData passwordData )
        {
            return new DefaultPasswordManagerStorage( passwordData );
        }
    }
}