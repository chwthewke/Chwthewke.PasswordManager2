using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordEditor
    {
        string Key { get; set; }

        void Reset( );

        void GeneratePasswords( byte[ ] masterPassword );

        IEnumerable<IPasswordGenerator> PasswordSlots { get; }

        IPasswordGenerator SavedSlot { get; }

        IPasswordDocument GeneratedPassword( IPasswordGenerator slot );
    }
}