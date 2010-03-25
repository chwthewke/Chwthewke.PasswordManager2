using System;

namespace Chwthewke.PasswordManager.Storage
{
    internal class TimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}