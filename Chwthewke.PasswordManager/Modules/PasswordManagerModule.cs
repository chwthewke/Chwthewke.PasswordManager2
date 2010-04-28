using System;
using System.Text;
using Autofac;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class PasswordManagerModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Editor

            builder.RegisterType<PasswordEditor>( ).As<IPasswordEditor>( );
            builder.RegisterType<PasswordEditorController>( ).As<IPasswordEditorController>( );
            // Engine
            builder.RegisterType<PasswordDigester>( ).As<IPasswordDigester>( );

            builder.RegisterType<Sha512Factory>( ).As<IHashFactory>( );
            
            builder.RegisterType<TimeProvider>( ).As<ITimeProvider>( );

            builder.RegisterInstance( PasswordGenerators.AlphaNumeric ).As<IPasswordGenerator>( );
            builder.RegisterInstance( PasswordGenerators.Full ).As<IPasswordGenerator>( );

            // Storage

            builder.RegisterInstance<Func<Guid>>( ( ) => Guid.NewGuid( ) ).As<Func<Guid>>( );

            builder.RegisterType<PasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );
            builder.RegisterInstance( new UTF8Encoding( false ) ).As<Encoding>( );
            builder.RegisterType<PasswordStoreSerializer>( ).As<IPasswordStoreSerializer>( );
        }
    }
}