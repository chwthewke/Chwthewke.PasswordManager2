using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class ApplicationServices : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder
                .RegisterInstance( new GuidToColorConverter( 0.6d, 1.0d, 0.4d, 0.8d ) )
                .As<IGuidToColorConverter>( );

            builder.RegisterType<ClipboardService>( ).As<IClipboardService>( );

            builder.RegisterType<DialogFileSelectionService>( ).As<IFileSelectionService>( );

            builder.RegisterType<PasswordEditorFactory>( ).As<IPasswordEditorFactory>( );
        }
    }
}