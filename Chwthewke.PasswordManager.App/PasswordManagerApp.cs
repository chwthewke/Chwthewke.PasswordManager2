using System;
using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;

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
            _passwordList = passwordManagerWindow.ViewModel.PasswordList;
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
            //SetupPersistence( );
            Run( );
        }
        //

        private void SetupPersistence( )
        {
            _passwordList.UpdateList( );
        }

        private readonly PasswordListViewModel2 _passwordList;
    }
}