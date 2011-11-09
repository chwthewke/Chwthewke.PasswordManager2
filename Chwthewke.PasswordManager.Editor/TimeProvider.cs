using System;

namespace Chwthewke.PasswordManager.Editor
{
    internal class TimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}