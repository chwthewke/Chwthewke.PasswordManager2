using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordManagerStorage
    {
        public PasswordManagerStorage( ITextResource passwordsResource )
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
}