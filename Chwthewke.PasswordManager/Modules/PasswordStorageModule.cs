using Autofac;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class PasswordStorageModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Storage
            builder.RegisterType<MasterPasswordMatcher>( ).As<IMasterPasswordMatcher>( ).SingleInstance( );

            builder.RegisterType<PasswordDatabase>( ).As<IPasswordDatabase>( ).SingleInstance( );
            builder.RegisterType<PasswordSerializer>( ).As<IPasswordSerializer>( ).SingleInstance( );

            builder.RegisterType<PasswordRepository>( ).As<IPasswordRepository>( ).SingleInstance( );
            builder.RegisterType<PersistenceService>( ).As<IPersistenceService>( );

        }
    }
}