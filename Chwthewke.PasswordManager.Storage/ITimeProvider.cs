using System;

namespace Chwthewke.PasswordManager.Storage
{
    [ Obsolete ]
    public interface ITimeProvider
    {
        DateTime Now { get; }
    }
}