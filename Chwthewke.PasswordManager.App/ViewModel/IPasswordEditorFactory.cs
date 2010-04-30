using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using System.Linq;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public interface IPasswordEditorFactory
    {
        PasswordEditorViewModel CreatePasswordEditor( IPasswordEditorController controller );
    }

    class PasswordEditorFactory : IPasswordEditorFactory {
        public PasswordEditorFactory( IClipboardService clipboardService, IEnumerable<IPasswordGenerator> generators )
        {
            this._clipboardService = clipboardService;
            _generators = generators;
        }

        public PasswordEditorViewModel CreatePasswordEditor( IPasswordEditorController controller )
        {
            return new PasswordEditorViewModel( controller, _clipboardService,
               _generators.Select( g => new PasswordSlotViewModel( g ) ) );
        }

        private readonly IEnumerable<IPasswordGenerator> _generators;
        private readonly IClipboardService _clipboardService;
    }
}