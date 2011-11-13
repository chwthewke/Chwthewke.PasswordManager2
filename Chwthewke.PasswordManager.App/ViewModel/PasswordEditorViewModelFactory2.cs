using System;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModelFactory2
    {
        public PasswordEditorViewModelFactory2( IClipboardService clipboardService, IGuidToColorConverter guidToColor,
                                                IPasswordManagerEditor editor )
        {
            _clipboardService = clipboardService;
            _guidToColor = guidToColor;
            _editor = editor;
        }

        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColor;
        private readonly IPasswordManagerEditor _editor;

        public PasswordEditorViewModel2 NewPasswordEditor( )
        {
            return CreateEditorViewModel( null );
        }

        public PasswordEditorViewModel2 PasswordEditorFor( PasswordDigestDocument document )
        {
            if ( document == null )
                throw new ArgumentNullException( "document" );
            return CreateEditorViewModel( document );
        }

        private PasswordEditorViewModel2 CreateEditorViewModel( PasswordDigestDocument document )
        {
            IPasswordEditorModel model = document == null ? _editor.EmptyModel( ) : _editor.ModelFor( document );
            return new PasswordEditorViewModel2( model, _clipboardService, _guidToColor );
        }
    }
}