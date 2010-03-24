using System.Collections.Generic;
using System.Security;

namespace Chwthewke.PasswordManager.Migration
{
    public interface ILegacyItemImporter
    {
        int NumPasswords { get; }
        IEnumerable<string> PasswordKeys { get; }
        void Import( IEnumerable<LegacyItem> items, SecureString masterPassword );
        void Save( string fileName );
    }
}