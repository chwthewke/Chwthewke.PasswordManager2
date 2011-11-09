using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Editor
{
    public class StubTimeProvider : ITimeProvider
    {
        public DateTime Now { get; set; }
    }
}