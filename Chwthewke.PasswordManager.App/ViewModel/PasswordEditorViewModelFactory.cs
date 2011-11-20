using System;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModelFactory
    {
        public PasswordEditorViewModelFactory( IClipboardService clipboardService,
                                               IDialogService dialogService,
                                               IGuidToColorConverter guidToColor,
                                               IPasswordManagerEditor editor )
        {
            _clipboardService = clipboardService;
            _dialogService = dialogService;
            _guidToColor = guidToColor;
            _editor = editor;
        }

        private readonly IClipboardService _clipboardService;
        private readonly IDialogService _dialogService;
        private readonly IGuidToColorConverter _guidToColor;
        private readonly IPasswordManagerEditor _editor;

        public PasswordEditorViewModel NewPasswordEditor( )
        {
            return CreateEditorViewModel( null );
        }

        public PasswordEditorViewModel PasswordEditorFor( PasswordDigestDocument document )
        {
            if ( document == null )
                throw new ArgumentNullException( "document" );
            return CreateEditorViewModel( document );
        }

        private PasswordEditorViewModel CreateEditorViewModel( PasswordDigestDocument document )
        {
            IPasswordEditorModel model = document == null ? _editor.EmptyModel( ) : _editor.ModelFor( document );
            return new PasswordEditorViewModel( model, _clipboardService, _dialogService, _guidToColor );
        }
    }
}