using System;
using Autofac;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Modules
{
    [ Obsolete ]
    public class ApplicationModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<PasswordManagerViewModel>( );
            builder.RegisterType<PasswordListViewModel>( );
            builder.RegisterType<PasswordEditorViewModelFactory>( );
            builder.RegisterType<StoredPasswordViewModel>( );


        }
    }
}