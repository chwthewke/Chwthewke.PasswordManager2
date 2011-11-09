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
        internal DefaultPasswordManagerStorage( ITextResource passwordsResource )
        {
            _passwordRepository = new PasswordRepository( new XmlPasswordData( new PasswordSerializer2(  ), passwordsResource ) );
        }

        public IMasterPasswordMatcher MasterPasswordMatcher
        {
            get { return new MasterPasswordMatcher2( PasswordManagerEngine.DerivationEngine, null ); }
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
            return new DefaultPasswordManagerStorage( passwordsResource );
        }
    }
}