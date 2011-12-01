using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.Services
{
    public static class ErrorDialogs
    {
        public static void ShowFileError( this IDialogService dialogService, string message )
        {
            dialogService.ShowErrorDialog( message, FileError );
        }

        public const string FileError = "File Error";
    }
}