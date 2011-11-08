using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorModelFactory : IPasswordEditorModelFactory
    {
        public IPasswordEditorModel CreatePrisineModel( )
        {
            return new PasswordEditorModel( _passwordCollection, _derivationEngine, _masterPasswordMatcher );
        }

        public IPasswordEditorModel CreateModel( PasswordDigestDocument password )
        {
            return new PasswordEditorModel( _passwordCollection, _derivationEngine, _masterPasswordMatcher, password );
        }

        private readonly IPasswordCollection _passwordCollection;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;

        public PasswordEditorModelFactory( IPasswordCollection passwords, IPasswordDerivationEngine derivationEngine )
        {
            _passwordCollection = passwords;
            _derivationEngine = derivationEngine;
            _masterPasswordMatcher = new MasterPasswordMatcher2( _derivationEngine, _passwordCollection );
        }
    }
}