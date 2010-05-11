using Autofac;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder
                .Register( c => new SettingsPasswordsDatabase( Settings.Default,
                                                               c.Resolve<IPasswordStore>( ),
                                                               c.Resolve<IPasswordStoreSerializer>( ) ) )
                .As<IPasswordPersistenceService>( );

            builder.RegisterType<PasswordManagerApp>( );

            builder.Register( c => new PasswordManagerWindow( c.Resolve<PasswordManagerViewModel>( ) ) );
            builder.RegisterType<PasswordManagerViewModel>( );
            builder.RegisterType<PasswordListViewModel>( );
        }
    }
}