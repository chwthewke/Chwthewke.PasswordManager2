using System;
using System.Security;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditorController
    {
        string Key { get; set; }
        string Note { get; set; }

        SecureString MasterPassword { get; set; }
        Guid? MasterPasswordId { get; set; }

        bool IsPasswordLoaded { get; set; }
        bool IsDirty { get; set; }

        void LoadPassword( );
        void UnloadPassword( );

        void SavePassword( );
        void DeletePassword( );
    }
}