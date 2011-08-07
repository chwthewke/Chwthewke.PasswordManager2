using Autofac;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class UninitializedPasswordStorageModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Storage
            builder.RegisterType<MasterPasswordMatcher>( ).As<IMasterPasswordMatcher>( ).SingleInstance( );

            builder.RegisterType<PasswordRepository>( ).As<IPasswordRepository>( ).SingleInstance( );
            builder.RegisterType<PasswordSerializer>( ).As<IPasswordSerializer>( );

            builder.RegisterType<PersistenceService>( ).As<IPersistenceService>( );

        }
    }
}