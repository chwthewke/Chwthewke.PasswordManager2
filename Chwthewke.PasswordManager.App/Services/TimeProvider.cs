using System;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class TimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}