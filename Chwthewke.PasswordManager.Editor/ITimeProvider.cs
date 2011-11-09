using System;

namespace Chwthewke.PasswordManager.Editor
{
    internal interface ITimeProvider
    {
        DateTime Now { get; }
    }
}