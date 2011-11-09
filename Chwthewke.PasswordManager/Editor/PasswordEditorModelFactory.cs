using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorModelFactory : IPasswordEditorModelFactory
    {
        public IPasswordEditorModel CreatePrisineModel( )
        {
            return new PasswordEditorModel( _passwordCollection, _derivationEngine, _masterPasswordMatcher, _timeProvider );
        }

        public IPasswordEditorModel CreateModel( PasswordDigestDocument password )
        {
            return new PasswordEditorModel( _passwordCollection, _derivationEngine, _masterPasswordMatcher, _timeProvider, password );
        }

        private readonly IPasswordCollection _passwordCollection;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly ITimeProvider _timeProvider;

        public PasswordEditorModelFactory( IPasswordCollection passwords, IPasswordDerivationEngine derivationEngine, ITimeProvider timeProvider )
        {
            _passwordCollection = passwords;
            _derivationEngine = derivationEngine;
            _timeProvider = timeProvider;
            _masterPasswordMatcher = new MasterPasswordMatcher2( _derivationEngine, _passwordCollection );
        }
    }
}