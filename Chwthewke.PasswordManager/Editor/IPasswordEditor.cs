using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditor
    {
        string Key { get; set; }
        string Note { get; set; }

        void Reset( );

        void GeneratePasswords( SecureString masterPassword );

        IEnumerable<IPasswordGenerator> PasswordSlots { get; }

        PasswordDocument GeneratedPassword( IPasswordGenerator slot );

        IPasswordGenerator SavedSlot { get; }
    }
}