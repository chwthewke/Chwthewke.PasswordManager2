using System;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public interface IDerivedPasswordModel
    {
        Guid Generator { get; }

        DerivedPassword DerivedPassword { get; }

        int Iteration { get; set; }
    }
}