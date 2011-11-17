using Autofac;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ApplicationServices : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterInstance( GuidToColorConverter ).As<IGuidToColorConverter>( );

            builder.RegisterType<ClipboardService>( ).As<IClipboardService>( );
            builder.RegisterType<DialogFileSelectionService>( ).As<IFileSelectionService>( );

            builder.RegisterType<PasswordExchange2>( ).As<IPasswordExchange>( );
            builder.RegisterType<FuzzyDateFormatter>( ).As<IFuzzyDateFormatter>( );
            builder.RegisterType<TimeProvider>( ).As<ITimeProvider>( );

            builder.RegisterInstance( Settings.Default ).As<Settings>( );
            builder.RegisterType<StorageConfiguration>( ).As<IStorageConfiguration>( ).SingleInstance( );

            builder.RegisterType<SingleInstanceManager>( );
            builder.RegisterType<PasswordManagerApp>( );

        }

        private static GuidToColorConverter GuidToColorConverter
        {
            get { return new GuidToColorConverter( 0.6d, 1.0d, 0.4d, 0.8d ); }
        }
    }
}