using System;
using Autofac;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ApplicationServices : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterInstance( GuidToColorConverter ).As<IGuidToColorConverter>( );

            builder.RegisterType<PasswordStoreProvider>( );
            builder.Register<Func<IPasswordStore>>( c => c.Resolve<PasswordStoreProvider>( ).GetPasswordStore )
                .SingleInstance( );

            builder.RegisterType<ClipboardService>( ).As<IClipboardService>( );
            builder.RegisterType<DialogFileSelectionService>( ).As<IFileSelectionService>( );

            builder.RegisterType<PasswordEditorFactory>( ).As<IPasswordEditorFactory>( );

            builder.RegisterType<PasswordImporter>( ).As<IPasswordImporter>( );
        }

        private static GuidToColorConverter GuidToColorConverter
        {
            get { return new GuidToColorConverter( 0.6d, 1.0d, 0.4d, 0.8d ); }
        }
    }
}