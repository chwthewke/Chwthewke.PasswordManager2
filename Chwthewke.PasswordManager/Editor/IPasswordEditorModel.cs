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

        IList<IDerivedPasswordModel> DerivedPasswords { get; }

        IDerivedPasswordModel SelectedPassword { get; set; }

        Guid? MasterPasswordId { get; }
        Guid? ExpectedMasterPasswordId { get; }

        bool IsDirty { get; }
        bool CanSave { get; }
        bool CanDelete { get; }
        string Note { get; set; }
        bool IsKeyReadonly { get; }

        bool Save( );
        bool Delete( );
    }
}