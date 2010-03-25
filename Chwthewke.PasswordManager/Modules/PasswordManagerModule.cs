using System.Text;
using Autofac;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class PasswordManagerModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Engine
            builder.RegisterType<PasswordDigester>( ).As<IPasswordDigester>( );

            builder.RegisterType<Sha512Factory>( ).As<IHashFactory>( );
            
            builder.RegisterType<TimeProvider>( ).As<ITimeProvider>( );

            builder.RegisterInstance( PasswordGenerators.AlphaNumeric ).As<IPasswordGenerator>( );
            builder.RegisterInstance( PasswordGenerators.Full ).As<IPasswordGenerator>( );

            // Storage
            builder.RegisterType<PasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );

            builder.RegisterInstance( new UTF8Encoding( false ) ).As<Encoding>( );

            builder.RegisterType<PasswordStoreSerializer>( ).As<IPasswordStoreSerializer>( );
        }
    }
}