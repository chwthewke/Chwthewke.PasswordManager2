﻿using System;

namespace Chwthewke.PasswordManager.Storage
{
    [ Obsolete ]
    internal class TimeProvider : ITimeProvider
    {
        public DateTime Now
        {
            get { return DateTime.Now; }
        }
    }
}