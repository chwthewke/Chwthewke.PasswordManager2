using System;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    [Obsolete]
    public interface IPasswordEditorFactory
    {
        PasswordEditorViewModel CreatePasswordEditor( );
    }
}