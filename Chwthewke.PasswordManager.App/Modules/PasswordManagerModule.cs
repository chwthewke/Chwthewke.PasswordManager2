using System;
using Autofac;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class PasswordManagerModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Controller
            builder.RegisterType<PasswordEditorControllerFactory>( );

            // Engine
            builder.RegisterType<PasswordDigester>( ).As<IPasswordDigester>( ).SingleInstance( );

            builder.RegisterType<Sha512Factory>( ).As<IHashFactory>( ).SingleInstance( );
            builder.RegisterType<TimeProvider>( ).As<ITimeProvider>( ).SingleInstance( );

            builder.RegisterInstance( PasswordGenerators.AlphaNumeric ).As<IPasswordGenerator>( );
            builder.RegisterInstance( PasswordGenerators.Full ).As<IPasswordGenerator>( );

            builder.RegisterInstance<Func<Guid>>( Guid.NewGuid ).As<Func<Guid>>( );
        }
    }
}