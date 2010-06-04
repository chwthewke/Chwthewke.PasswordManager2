using Autofac;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class UninitializedPasswordStorageModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Storage
            builder.RegisterType<PasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );
            builder.RegisterType<PasswordStoreSerializer>( ).As<IPasswordStoreSerializer>( );
        }
    }
}