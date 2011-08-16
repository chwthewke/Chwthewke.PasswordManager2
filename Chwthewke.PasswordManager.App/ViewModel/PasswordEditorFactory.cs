using System;
using System.Linq;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    internal class PasswordEditorFactory : IPasswordEditorFactory
    {
        public PasswordEditorFactory( IClipboardService clipboardService,
                                      Func<IPasswordEditorController> controllerFactory,
                                      IGuidToColorConverter guidToColorConverter )
        {
            _clipboardService = clipboardService;
            _guidToColorConverter = guidToColorConverter;
            _controllerFactory = controllerFactory;
        }

        public PasswordEditorViewModel CreatePasswordEditor( )
        {
            IPasswordEditorController controller = _controllerFactory.Invoke( );
            return new PasswordEditorViewModel( controller, _clipboardService,
                                                controller.Generators.Select( g => new PasswordSlotViewModel( g ) ),
                                                _guidToColorConverter );
        }

        private readonly Func<IPasswordEditorController> _controllerFactory;
        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColorConverter;
    }
}