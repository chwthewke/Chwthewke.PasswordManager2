using System;
using System.Collections.Generic;
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
        Guid? ExpectedMasterPasswordId { get; }

        bool IsKeyStored { get; }
        bool IsPasswordLoaded { get; }
        bool IsDirty { get; }

        IEnumerable<IPasswordGenerator> Generators { get; }
        IPasswordGenerator SelectedGenerator { get; set; }
        string GeneratedPassword( IPasswordGenerator generator );

        void LoadPassword( );
        void UnloadPassword( );

        void SavePassword( );
        void DeletePassword( );
    }
}