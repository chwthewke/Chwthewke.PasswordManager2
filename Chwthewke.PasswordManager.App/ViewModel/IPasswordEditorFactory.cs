using System;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public interface IPasswordEditorFactory
    {
        PasswordEditorViewModel CreatePasswordEditor( );
    }
}