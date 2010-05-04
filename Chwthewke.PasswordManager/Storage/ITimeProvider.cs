using System;

namespace Chwthewke.PasswordManager.Storage
{
    internal interface ITimeProvider
    {
        DateTime Now { get; }
    }
}