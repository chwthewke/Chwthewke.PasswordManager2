using System;
using System.Collections.Generic;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface IExclusiveDelayedScheduler
    {
        void ScheduleActions( double delay, IEnumerable<Action> actions );
    }
}