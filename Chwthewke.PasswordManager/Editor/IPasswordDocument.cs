using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordDocument
    {
        string Key { get; set; }

        IEnumerable<Pair<string, PasswordInfo>> GeneratedPasswords { get; }

        // prolly GeneratePasswords here actually

        PasswordInfo SavedPasswordInfo { get; }
    }
}