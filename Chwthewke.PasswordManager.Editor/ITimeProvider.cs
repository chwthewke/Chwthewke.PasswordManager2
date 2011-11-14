using System;

namespace Chwthewke.PasswordManager.Editor
{
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}