using System;
using System.Linq;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;

namespace Chwthewke.PasswordManager.Test.App
{
    [Obsolete]
    internal class AppSetUp
    {
        // TODO clean up API
        public static IContainer TestContainer( params Module[ ] modules )
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