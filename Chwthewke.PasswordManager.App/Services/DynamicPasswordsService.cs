using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IPasswordsService
    {
        IPasswordRepository PasswordRepository { get; }
        IMasterPasswordMatcher MasterPasswordMatcher { get; }
        IPasswordEditorModel EmptyModel( );
        IPasswordEditorModel ModelFor( PasswordDigestDocument password );
        void SetupXmlDataSource( ITextResource textResource );
        void SetupDataSource( IPasswordData passwordData );
    }

    public class DynamicPasswordsService : IPasswordsService
    {
        public IPasswordEditorModel EmptyModel( )
        {
            return _editor.EmptyModel( );
        }

        public IPasswordEditorModel ModelFor( PasswordDigestDocument password )
        {
            return _editor.ModelFor( password );
        }

        public IMasterPasswordMatcher MasterPasswordMatcher
        {
            get { return _storage.MasterPasswordMatcher; }
        }

        public IPasswordRepository PasswordRepository
        {
            get { return _storage.PasswordRepository; }
        }


        public void SetupXmlDataSource( ITextResource textResource )
        {
            Update( PasswordManagerStorage.CreateService( textResource ) );
        }

        public void SetupDataSource( IPasswordData passwordData )
        {
            Update( PasswordManagerStorage.CreateService( passwordData ) );
        }

        private void Update( IPasswordManagerStorage storage )
        {
            // TODO Huh, merge here ?
            _storage = storage;
            _editor = PasswordManagerEditor.CreateEditor( PasswordManagerEngine.DerivationEngine, _storage );
        }

        private IPasswordManagerStorage _storage;
        private IPasswordManagerEditor _editor;
    }
}