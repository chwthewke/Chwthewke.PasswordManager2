using System;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App
{
    internal class AppSetUp
    {
        // TODO clean up API
        public static IContainer TestContainer( params Module[] modules )
        {
            return TestContainer( b => { }, modules );
        }

        public static IContainer TestContainer( Action<ContainerBuilder> extraSetup,
            params Module[ ] modules )
        {
            ContainerBuilder builder = new ContainerBuilder( );

            foreach ( Module module in AppConfiguration.ApplicationModules.Concat( modules ) )
                builder.RegisterModule( module );

            extraSetup( builder );

            return builder.Build( );
        }
    }
}