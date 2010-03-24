using System;
using System.Reflection;
using Autofac;
using Module = Autofac.Module;

namespace Chwthewke.PasswordManager.Migration
{
    public class MigrationModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.RegisterType<LegacyItemImporter>( ).As<ILegacyItemImporter>( );
            builder.RegisterType<LegacyItemLoader>( ).As<ILegacyItemLoader>( );

            builder.RegisterAssemblyTypes( Assembly.GetExecutingAssembly( ) )
                .Where( t => t.IsClass );
        }
    }
}