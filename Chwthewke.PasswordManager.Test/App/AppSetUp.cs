using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App
{
    internal class AppSetUp
    {
        public static IContainer TestContainer( params Module[] modules )
        {
            ContainerBuilder builder = new ContainerBuilder( );

            foreach ( Module module in 
                AppConfiguration.ApplicationModules.Concat( modules ) )

                builder.RegisterModule( module );

            return builder.Build( );
        }
    }
}