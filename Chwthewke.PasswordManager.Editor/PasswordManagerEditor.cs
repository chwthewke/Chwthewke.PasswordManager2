using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordManagerEditor
    {
        IPasswordEditorModel EmptyModel( );
        IPasswordEditorModel ModelFor( PasswordDigestDocument password );
    }

    public static class PasswordManagerEditor
    {
        public static IPasswordManagerEditor CreateEditor( IPasswordDerivationEngine derivationEngine, IPasswordManagerStorage passwordManagerStorage )
        {
            return new DefaultPasswordManagerEditor( derivationEngine, passwordManagerStorage );
        }
    }

    internal class DefaultPasswordManagerEditor : IPasswordManagerEditor
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

        internal DefaultPasswordManagerEditor( IPasswordDerivationEngine derivationEngine,
                                      IPasswordManagerStorage passwordManagerStorage )
        {
            _passwordRepository = passwordManagerStorage.PasswordRepository;
            _derivationEngine = derivationEngine;
            _masterPasswordMatcher = passwordManagerStorage.MasterPasswordMatcher;
            _timeProvider = new TimeProvider( );
        }
    }
}