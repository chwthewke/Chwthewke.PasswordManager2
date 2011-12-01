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
        public static IPasswordManagerEditor CreateService( IPasswordDerivationEngine derivationEngine,
                                                            IPasswordManagerStorage passwordManagerStorage, ITimeProvider timeProvider )
        {
            return new DefaultPasswordManagerEditor( derivationEngine, passwordManagerStorage, timeProvider );
        }
    }

    internal class DefaultPasswordManagerEditor : IPasswordManagerEditor
    {
        public IPasswordEditorModel EmptyModel( )
        {
            return new PasswordEditorModel( _passwordRepository, _derivationEngine, _masterPasswordMatcher, _timeProvider,
                                            new NewPasswordDocument( ) );
        }

        public IPasswordEditorModel ModelFor( PasswordDigestDocument password )
        {
            return new PasswordEditorModel( _passwordRepository, _derivationEngine, _masterPasswordMatcher, _timeProvider,
                                            new BaselinePasswordDocument( password ) );
        }

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly ITimeProvider _timeProvider;

        internal DefaultPasswordManagerEditor( IPasswordDerivationEngine derivationEngine,
                                               IPasswordManagerStorage passwordManagerStorage,
                                               ITimeProvider timeProvider )
        {
            _passwordRepository = passwordManagerStorage.PasswordRepository;
            _derivationEngine = derivationEngine;
            _timeProvider = timeProvider;
            _masterPasswordMatcher = passwordManagerStorage.MasterPasswordMatcher;
        }
    }
}