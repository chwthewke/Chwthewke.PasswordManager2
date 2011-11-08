using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorModelFactory : IPasswordEditorModelFactory
    {
        public IPasswordEditorModel CreateModel( PasswordDigestDocument password )
         {
             return new PasswordEditorModel( password, _passwordCollection, _derivationEngine, _masterPasswordMatcher );
         }

        private readonly IPasswordCollection _passwordCollection;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;

        public PasswordEditorModelFactory( IPasswordCollection passwords )
        {
            _passwordCollection = passwords;
            _derivationEngine = new PasswordDerivationEngine( PasswordGenerators2.Generators );
            _masterPasswordMatcher = new MasterPasswordMatcher2( _derivationEngine, _passwordCollection );
        }
    }
}