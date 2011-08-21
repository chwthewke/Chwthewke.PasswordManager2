using System.Collections.Generic;
using Autofac;
using Chwthewke.PasswordManager.Modules;

namespace Chwthewke.PasswordManager.App.Modules
{
    public static class AppConfiguration
    {
        public static IContainer ConfigureContainer( )
        {
            ContainerBuilder builder = new ContainerBuilder( );

            foreach ( Module module in ApplicationModules )
                builder.RegisterModule( module );

            return builder.Build( );
        }

        public static IEnumerable<Module> ApplicationModules
        {
            get
            {
                return new List<Module>
                           {
                               new PasswordManagerModule( ),
                               new PasswordStorageModule( ),
                               new ApplicationServices( ),
                               new ApplicationModule( ),
                           };
            }
        }
    }
}