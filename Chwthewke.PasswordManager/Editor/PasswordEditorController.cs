using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorController : IPasswordEditorController
    {
        public PasswordEditorController( )
        {
            Key = string.Empty;
            Note = string.Empty;
            MasterPassword = new SecureString( );
            GeneratedPassword = string.Empty;
        }

        public string Key { get; set; }

        public string Note { get; set; }

        public SecureString MasterPassword { get; set; }

        public Guid? MasterPasswordId { get; private set; }

        public bool IsPasswordLoaded { get; private set; }

        public bool IsDirty { get; private set; }

        public IPasswordGenerator Generator { get; set; }

        public string GeneratedPassword { get; private set; }

        public void LoadPassword( )
        {
            throw new NotImplementedException( );
        }

        public void UnloadPassword( )
        {
            throw new NotImplementedException( );
        }

        public void SavePassword( )
        {
            throw new NotImplementedException( );
        }

        public void DeletePassword( )
        {
            throw new NotImplementedException( );
        }
    }
}