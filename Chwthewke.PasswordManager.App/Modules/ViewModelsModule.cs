using Autofac;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ViewModelsModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<PasswordManagerViewModel>( );

            builder.RegisterType<PasswordListViewModel>( );
            builder.RegisterType<PasswordListEntryViewModel>( );

            builder.RegisterType<PasswordEditorViewModel>( );
            builder.RegisterType<PasswordEditorViewModelFactory>( );
            builder.RegisterType<DerivedPasswordViewModel>( );

            builder.Register( c => new PasswordManagerWindow( c.Resolve<PasswordManagerViewModel>( ) ) )
                .As<PasswordManagerWindow>( )
                .SingleInstance( );
        }
    }
}