using System.Windows;

namespace Chwthewke.PasswordManager.Migration
{
    public class MigrationApp : Application
    {
        public static void Main( string[ ] args )
        {
            Application app = new MigrationApp { MainWindow = new MainWindow( ) };
            app.Run( app.MainWindow );
        }
    }
}