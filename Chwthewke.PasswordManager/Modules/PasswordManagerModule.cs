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
            builder.RegisterType<PasswordStore>( ).As<IPasswordStore>( );
            builder.RegisterType<MasterPasswordFinder>( ).As<IMasterPasswordFinder>( );
            builder.RegisterType<MasterPasswordMatcher>( ).As<IMasterPasswordMatcher>( );
            builder.RegisterType<PasswordDigester>( ).As<IPasswordDigester>( );


            builder.RegisterType<Sha512Factory>( ).As<IHashFactory>( );

            builder.RegisterType<TimeProvider>( ).As<ITimeProvider>( );


            builder.RegisterInstance( new UTF8Encoding( false ) ).As<Encoding>( );
            builder.RegisterType<PasswordStoreSerializer>( ).As<IPasswordStoreSerializer>( );
        }
    }
}