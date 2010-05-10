using System.Text;
using Autofac;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Modules
{
    public class UninitializedPasswordStorage : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Storage
            builder.RegisterType<PasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );
            builder.RegisterType<PasswordStoreSerializer>( ).As<IPasswordStoreSerializer>( );

            builder.RegisterInstance( new UTF8Encoding( false ) ).As<Encoding>( );
        }
    }
}