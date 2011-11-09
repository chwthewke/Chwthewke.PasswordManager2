using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public class PasswordManagerEditor
    {
        public IPasswordEditorModel EmptyModel( )
        {
            return new PasswordEditorModel( _passwordRepository, _derivationEngine, _masterPasswordMatcher, _timeProvider );
        }

        public IPasswordEditorModel ModelFor( PasswordDigestDocument password )
        {
            return new PasswordEditorModel( _passwordRepository, _derivationEngine, _masterPasswordMatcher, _timeProvider, password );
        }

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly ITimeProvider _timeProvider;

        public PasswordManagerEditor( IPasswordRepository passwords, 
            IPasswordDerivationEngine derivationEngine, 
            IMasterPasswordMatcher masterPasswordMatcher )
        {
            _passwordRepository = passwords;
            _derivationEngine = derivationEngine;
            _masterPasswordMatcher = masterPasswordMatcher;
            _timeProvider = new TimeProvider( );
        }
    }
}