using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    [ Obsolete ]
    public interface IPasswordEditorController
    {
        string Key { get; set; }
        string Note { get; set; }

        SecureString MasterPassword { get; set; }
        Guid? MasterPasswordId { get; }
        Guid? ExpectedMasterPasswordId { get; }

        bool IsPasswordLoaded { get; }
        bool IsSaveable { get; }

        IEnumerable<IPasswordGenerator> Generators { get; }
        IPasswordGenerator SelectedGenerator { get; set; }
        string SelectedPassword { get; }
        string GeneratedPassword( IPasswordGenerator generator );

        void SavePassword( );
        void DeletePassword( );
        void ReloadBaseline( );
        void IncreaseIteration( IPasswordGenerator generator );
        void DecreaseIteration( IPasswordGenerator generator );
    }
}