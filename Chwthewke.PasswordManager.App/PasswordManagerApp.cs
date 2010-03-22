using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.App.Configuration;

namespace Chwthewke.PasswordManager.App
{
    public class PasswordManagerApp : Application
    {
        public static void Main( string[ ] args )
        {
            IContainer container = new AppConfiguration( ).ConfigureContainer( );
            PasswordManagerApp passwordManagerApp = container.Resolve<PasswordManagerApp>( );
            passwordManagerApp.Run( passwordManagerApp.MainWindow );
        }

        public PasswordManagerApp( MainWindow mainWindow )
        {
            MainWindow = mainWindow;
        }
    }
}