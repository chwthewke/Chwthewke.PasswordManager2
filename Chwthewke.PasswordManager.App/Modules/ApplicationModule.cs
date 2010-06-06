using Autofac;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ApplicationModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterInstance( Settings.Default ).As<Settings>( );

            builder.RegisterType<PasswordManagerApp>( );

            builder.Register( c => new PasswordManagerWindow( c.Resolve<PasswordManagerViewModel>( ) ) );
            builder.RegisterType<PasswordManagerViewModel>( );
            builder.RegisterType<PasswordListViewModel>( );
        }
    }
}