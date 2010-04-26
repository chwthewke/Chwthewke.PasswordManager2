using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditorController
    {
        string Key { get; set; }
        string Note { get; set; }

        SecureString MasterPassword { get; set; }
        Guid? MasterPasswordId { get; }

        bool IsPasswordLoaded { get; }
        bool IsDirty { get; }

        IPasswordGenerator Generator { get; set; }
        string GeneratedPassword { get; }

        void LoadPassword( );
        void UnloadPassword( );

        void SavePassword( );
        void DeletePassword( );
    }
}