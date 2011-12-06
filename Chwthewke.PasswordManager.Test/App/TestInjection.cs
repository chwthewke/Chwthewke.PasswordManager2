using System;
using System.Collections.Generic;
using Autofac;
using Autofac.Core;
using Chwthewke.PasswordManager.App.Modules;
using System.Linq;
using Chwthewke.PasswordManager.App.Properties;
using Moq;

namespace Chwthewke.PasswordManager.Test.App
{
    public static class TestInjection
    {
        public static IModule ModuleOf( Action<ContainerBuilder> setup )
        {
            return new ActionModule( setup );
        }

        public static IModule Mock<T>( ) where T : class
        {
            return new MockModule<T>( );
        }

        public static IContainer TestContainer( params IModule[ ] userModules )
        {
            var containerBuilder = new ContainerBuilder( );

            IEnumerable<IModule> allModules = AppConfiguration.ApplicationModules
                .Concat( new[ ] { new SettingsModule( ) } )
                .Concat( userModules );

            foreach ( IModule module in allModules )
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

    internal class MockModule<T> : Module where T : class
    {
        protected override void Load( ContainerBuilder builder )
        {
            Mock<T> mock = new Mock<T>( );
            builder.RegisterInstance( mock ).As<Mock<T>>( );
            builder.RegisterInstance( mock.Object ).As<T>( );
        }
    }

    internal class SettingsModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterInstance( new Settings( ) ).As<Settings>( );
        }
    }
}