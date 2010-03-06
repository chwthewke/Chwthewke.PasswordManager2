using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IPasswordDocument
    {
        string GeneratedPassword { get; }

        PasswordInfo SavablePasswordInfo { get; }
    }
}