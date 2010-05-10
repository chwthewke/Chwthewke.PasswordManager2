using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;

namespace Chwthewke.PasswordManager.App
{
    public class PasswordManagerApp : Application
    {
        public static void Main( string[ ] args )
        {
            IContainer container = AppConfiguration.ConfigureContainer( );
            PasswordManagerApp passwordManagerApp = container.Resolve<PasswordManagerApp>( );
            passwordManagerApp.Run( passwordManagerApp.MainWindow );
        }

        public PasswordManagerApp( MainWindow mainWindow )
        {
            MainWindow = mainWindow;
        }
    }
}