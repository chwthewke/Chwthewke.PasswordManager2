using System;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.Test.Editor
{
    public class StubTimeProvider : ITimeProvider
    {
        public DateTime Now { get; set; }
    }
}