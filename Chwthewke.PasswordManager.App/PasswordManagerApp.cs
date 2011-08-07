using System;
using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.View;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Storage;

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

        public PasswordManagerApp( PasswordManagerWindow passwordManagerWindow,
                                   Func<IPersistenceService> persistenceServiceProvider )
        {
            MainWindow = passwordManagerWindow;
            _passwordList = passwordManagerWindow.ViewModel.PasswordList;
            _persistenceService = persistenceServiceProvider( );
        }

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
            SetupPersistence( );
            Run( );
        }

        private void SetupPersistence( )
        {
            _persistenceService.Init( );
            _passwordList.UpdateList( );
            _passwordList.SaveRequested += ( s, e ) => _persistenceService.Save( );
            Exit += ( s, e ) => _persistenceService.Save( );
        }

        private readonly IPersistenceService _persistenceService;
        private readonly PasswordListViewModel _passwordList;
    }
}