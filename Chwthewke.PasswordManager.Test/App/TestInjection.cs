using System;
using System.Collections.Generic;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App
{
    public static class TestInjection
    {
        public static Module ModuleOf( Action<ContainerBuilder> setup )
        {
            return new ActionModule( setup );
        }

        public static IContainer TestContainer( params Module[] modules )
        {
            ContainerBuilder containerBuilder = new ContainerBuilder( );
            
            foreach ( Module module in AppConfiguration.ApplicationModules2.Concat( modules ) )
                containerBuilder.RegisterModule( module );
            
            return containerBuilder.Build( );
        }
    }

    internal class ActionModule : Module
    {
        private readonly Action<ContainerBuilder> _setup;

        public ActionModule( Action<ContainerBuilder> setup )
        {
            _setup = setup;
        }

        protected override void Load( ContainerBuilder builder )
        {
            _setup( builder );
        }
    }
}