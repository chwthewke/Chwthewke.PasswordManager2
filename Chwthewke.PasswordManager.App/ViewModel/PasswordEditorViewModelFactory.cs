using System;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModelFactory
    {
        public PasswordEditorViewModelFactory( PasswordEditorViewModel.Factory factory,
                                               IPasswordManagerEditor editor )
        {
            _factory = factory;
            _editor = editor;
        }

        private readonly PasswordEditorViewModel.Factory _factory;
        private readonly IPasswordManagerEditor _editor;

        public PasswordEditorViewModel NewPasswordEditor( )
        {
            return _factory( _editor.EmptyModel( ) );
        }

        public PasswordEditorViewModel PasswordEditorFor( PasswordDigestDocument document )
        {
            if ( document == null )
                throw new ArgumentNullException( "document" );
            return _factory( _editor.ModelFor( document ) );
        }
    }
}