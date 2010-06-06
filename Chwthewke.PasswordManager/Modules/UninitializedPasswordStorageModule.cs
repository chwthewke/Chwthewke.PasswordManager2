using Autofac;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class UninitializedPasswordStorageModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Storage
            builder.RegisterType<PasswordRepository>( ).As<IPasswordRepository>( ).SingleInstance( );
            builder.RegisterType<PasswordSerializer>( ).As<IPasswordSerializer>( );
        }
    }
}