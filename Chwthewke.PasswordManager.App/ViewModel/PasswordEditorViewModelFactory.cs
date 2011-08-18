using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModelFactory
    {
        public PasswordEditorViewModelFactory( IClipboardService clipboardService, IGuidToColorConverter guidToColor, PasswordEditorControllerFactory controllerFactory )
        {
            _clipboardService = clipboardService;
            _guidToColor = guidToColor;
            _controllerFactory = controllerFactory;
        }

        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColor;
        private readonly PasswordEditorControllerFactory _controllerFactory;

        public PasswordEditorViewModel PasswordEditorFor( string passwordKey )
        {
            return new PasswordEditorViewModel( _controllerFactory.PasswordEditorControllerFor( passwordKey ),
                                                _clipboardService, _guidToColor );
        }
    }
}