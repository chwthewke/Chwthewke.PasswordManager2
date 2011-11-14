using Autofac;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ApplicationModule2 : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<PasswordManagerViewModel>( );

            builder.RegisterType<PasswordListViewModel2>( );
            builder.RegisterType<StoredPasswordViewModel2>( );

            builder.RegisterType<PasswordEditorViewModelFactory2>( );
        }
    }
}