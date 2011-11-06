using System;

namespace Chwthewke.PasswordManager.Storage
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}