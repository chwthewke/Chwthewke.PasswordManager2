using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Chwthewke.PasswordManager.App.Services;

namespace Chwthewke.PasswordManager.Test.App.Services
{
    public class ImmediateScheduler: IExclusiveDelayedScheduler
    {
        public void ScheduleActions( double delay, IEnumerable<Action> actions )
        {
            foreach ( var action in actions )
                action.Invoke( );
        }

        public static IModule Module
        {
            get { return TestInjection.ModuleOf( cb => cb.RegisterType<ImmediateScheduler>( ).As<IExclusiveDelayedScheduler>( ) ); }
        }
    }
}