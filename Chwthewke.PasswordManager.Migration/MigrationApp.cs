using System;
using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.Modules;

namespace Chwthewke.PasswordManager.Migration
{
    public class MigrationApp : Application
    {
        [ STAThread ]
        public static void Main( string[ ] args )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            builder.RegisterModule( new PasswordStorageModule( ) );
            builder.RegisterModule( new MigrationModule( ) );

            MigrationApp app = builder.Build( ).Resolve<MigrationApp>( );
            app.Run( app.MainWindow );
        }

        public MigrationApp( MainWindow mainWindow )
        {
            MainWindow = mainWindow;
        }
    }
}