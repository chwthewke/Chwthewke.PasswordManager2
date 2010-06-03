using System.Linq;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    internal class PasswordEditorFactory : IPasswordEditorFactory
    {
        public PasswordEditorFactory( IClipboardService clipboardService,
                                      IPasswordEditorControllerFactory controllerFactory,
                                      IGuidToColorConverter guidToColorConverter )
        {
            _clipboardService = clipboardService;
            _guidToColorConverter = guidToColorConverter;
            _controllerFactory = controllerFactory;
        }

        public PasswordEditorViewModel CreatePasswordEditor( )
        {
            IPasswordEditorController controller = _controllerFactory.CreatePasswordEditorController( );
            return new PasswordEditorViewModel( controller, _clipboardService,
                                                controller.Generators.Select( g => new PasswordSlotViewModel( g ) ),
                                                _guidToColorConverter );
        }

        private readonly IPasswordEditorControllerFactory _controllerFactory;
        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColorConverter;
    }
}