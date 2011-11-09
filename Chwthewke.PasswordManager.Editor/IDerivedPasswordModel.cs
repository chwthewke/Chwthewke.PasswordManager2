using System;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IDerivedPasswordModel
    {
        Guid Generator { get; }

        IDerivedPassword DerivedPassword { get; }
    }
}