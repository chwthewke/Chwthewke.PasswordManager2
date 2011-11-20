using System.Windows;
using Chwthewke.PasswordManager.App.Services;

namespace Chwthewke.PasswordManager.App.View
{
    public class DialogService : IDialogService
    {
        public void ShowErrorDialog( string message, string caption )
        {
            MessageBox.Show( message, caption, MessageBoxButton.OK, MessageBoxImage.Error );
        }
    }
}