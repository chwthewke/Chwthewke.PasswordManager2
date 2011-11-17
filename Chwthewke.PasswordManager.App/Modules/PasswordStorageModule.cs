﻿using Autofac;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class PasswordStorageModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            // Storage
            builder.RegisterType<MasterPasswordMatcher>( ).As<IMasterPasswordMatcher>( ).SingleInstance( );

            builder.RegisterType<PasswordDatabase>( ).As<IPasswordDatabase>( ).SingleInstance( );
            builder.RegisterType<PasswordSerializer>( ).As<IPasswordSerializer>( ).SingleInstance( );
            // default init
            builder.RegisterType<NullTextResource>( ).As<ITextResource>( ).SingleInstance( );
        }
    }
}