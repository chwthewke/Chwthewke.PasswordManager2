using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class PasswordManagerModule2 : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<PasswordManagerEditor>( );

            builder.RegisterInstance( PasswordManagerEngine.DerivationEngine ).As<IPasswordDerivationEngine>( );

            builder.Register( c => new PasswordManagerStorage( c.Resolve<ITextResource>( ) ) ).As<PasswordManagerStorage>( ).SingleInstance(  );

            builder.RegisterType<DynamicTextResource>( ).SingleInstance( );

            builder.RegisterType<DynamicTextResource>( ).As<ITextResource>( );
        }
    }
}