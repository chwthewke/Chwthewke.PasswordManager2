using System;
using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditorModel
    {
        string Key { get; set; }

        SecureString MasterPassword { get; set; }

        int Iteration { get; set; }

        void UpdateDerivedPasswords( );

        IEnumerable<IDerivedPasswordModel> DerivedPasswords { get; }

        IDerivedPasswordModel SelectedPassword { get; set; }

        string Note { get; set; }

        Guid? MasterPasswordId { get; }
        Guid? ExpectedMasterPasswordId { get; }

        void Reload( );

        bool IsKeyReadonly { get; }
        
        bool IsDirty { get; }

        bool CanSave { get; }
        bool Save( );

        bool CanDelete { get; }
        bool Delete( );

        
    }
}