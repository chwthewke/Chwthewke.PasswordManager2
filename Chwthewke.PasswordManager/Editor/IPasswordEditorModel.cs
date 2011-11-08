using System;
using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditorModel
    {
        string Key { get; set; }

        SecureString MasterPassword { get; set; }

        IList<IDerivedPasswordModel> DerivedPasswords { get; }

        IDerivedPasswordModel SelectedPassword { get; }

        Guid? MasterPasswordId { get; }
        Guid? ExpectedMasterPasswordId { get; }

        bool IsDirty { get; }
        bool CanSave { get; }
        bool CanDelete { get; }

        bool Save( );
        bool Delete( );
    }
}