using System;
using System.Windows;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using Chwthewke.PasswordManager.App.Services;
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
            PasswordManagerApp passwordManagerApp = container.Resolve<PasswordManagerApp>( );

            passwordManagerApp.Start( );
        }

        public PasswordManagerApp( PasswordManagerWindow passwordManagerWindow,
                                   Func<IPersistenceService> persistenceServiceProvider )
        {
            MainWindow = passwordManagerWindow;
            _passwordList = passwordManagerWindow.ViewModel.PasswordList;
            _persistenceService = persistenceServiceProvider( );
        }

        private void Start( )
        {
            SetupPersistence( );
            Run( MainWindow );
        }

        private void SetupPersistence( )
        {
            _persistenceService.Start( );
            _passwordList.UpdateList( );
            _passwordList.SaveRequested += ( s, e ) => _persistenceService.Save( );
            Exit += ( s, e ) => _persistenceService.Stop( );
        }

        private readonly IPersistenceService _persistenceService;
        private readonly PasswordListViewModel _passwordList;
    }
}