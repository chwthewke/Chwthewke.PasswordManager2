using Autofac;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ViewModelsModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<PasswordManagerViewModel2>( );

            builder.RegisterType<PasswordListViewModel2>( );
            builder.RegisterType<StoredPasswordViewModel2>( );

            builder.RegisterType<PasswordEditorViewModelFactory2>( );

            builder.Register( c => new PasswordManagerWindow( c.Resolve<PasswordManagerViewModel2>( ) ) )
                .As<PasswordManagerWindow>( )
                .SingleInstance( );
        }
    }
}