using System;
using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using Chwthewke.PasswordManager.App.View;

namespace Chwthewke.PasswordManager.App
{
    public class PasswordManagerApp : Application
    {
        [ STAThread ]
        public static void Main( string[ ] args )
        {
            IContainer container = AppConfiguration.ConfigureContainer( );
            SingleInstanceManager instanceManager = container.Resolve<SingleInstanceManager>( );

            instanceManager.Run( args );
        }

        public PasswordManagerApp( PasswordManagerWindow passwordManagerWindow )
        {
            MainWindow = passwordManagerWindow;
        }

        // Single instance app lifecycle management
        public void Activate( )
        {
            MainWindow.Activate( );
        }

        protected override void OnStartup( StartupEventArgs e )
        {
            base.OnStartup( e );

            MainWindow.Show( );
        }

        public void Start( )
        {
            Run( );
        }
    }
}