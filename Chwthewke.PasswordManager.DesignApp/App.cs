using System;
using System.Collections.ObjectModel;
using System.Windows;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.DesignApp
{
    public class App : Application
    {
        [STAThread]
        public static void Main( string[ ] args )
        {
            App app = new App( ) { MainWindow = new MainWindow( CreateDesignViewModel( ) ) };
            app.Run( app.MainWindow );
        }

        private static DesignViewModel CreateDesignViewModel( )
        {
            PasswordGroupingViewModel grouping = new PasswordGroupingViewModel( );

            grouping.PasswordId = Guid.NewGuid( ).ToString( );
            grouping.CreationTime = DateTime.Now;
            grouping.Passwords = new ObservableCollection<string>( new[] { "google", "amazon", "orange" } );

            return new DesignViewModel { PasswordGrouping = grouping };
        }
    }
}