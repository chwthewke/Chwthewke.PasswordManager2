using System.Windows;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App
{
    /// <summary>
    /// Interaction logic for PasswordManagerWindow.xaml
    /// </summary>
    public partial class PasswordManagerWindow
    {
        public PasswordManagerWindow( ) : this( null ) {}

        public PasswordManagerWindow( PasswordManagerViewModel viewModel )
        {
            if ( viewModel != null )
                ViewModel = viewModel;
            InitializeComponent( );
        }

        public PasswordManagerViewModel ViewModel
        {
            get { return DataContext as PasswordManagerViewModel; }
            set { DataContext = value; }
        }

        private void Button_Click( object sender, RoutedEventArgs e ) {}
    }
}