using Autofac;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Test.Storage;

namespace Chwthewke.PasswordManager.App.Modules
{
    public static class AppConfiguration
    {
        public static IContainer ConfigureContainer( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            builder.RegisterModule( new UninitializedPasswordStorageModule( ) );
            builder.RegisterModule( new ApplicationServices( ) );
            builder.RegisterModule( new ApplicationModule( ) );

            return builder.Build( );
        }
    }
}