﻿using Autofac;
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
            builder.Register( CreateEditor ).As<IPasswordManagerEditor>( ).SingleInstance( );

            builder.RegisterInstance( PasswordManagerEngine.DerivationEngine ).As<IPasswordDerivationEngine>( );

            builder.Register( CreateStorage ).As<IPasswordManagerStorage>( ).SingleInstance( );

            builder.RegisterType<DynamicTextResource>( ).SingleInstance( );

            builder.RegisterType<DynamicTextResource>( ).As<ITextResource>( );
        }

        private IPasswordManagerStorage CreateStorage( IComponentContext c )
        {
            return PasswordManagerStorage.CreateService( c.Resolve<ITextResource>( ) );
        }

        private IPasswordManagerEditor CreateEditor( IComponentContext c )
        {
            return PasswordManagerEditor.CreateEditor( c.Resolve<IPasswordDerivationEngine>( ), c.Resolve<IPasswordManagerStorage>( ) );
        }
    }
}