using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    ///   Interaction logic for PasswordManagerWindow.xaml
    /// </summary>
    public partial class PasswordManagerWindow
    {
        public PasswordManagerWindow( ) : this( null )
        {
        }

        public PasswordManagerWindow( PasswordManagerViewModel2 viewModel )
        {
            if ( viewModel != null )
                ViewModel = viewModel;
            InitializeComponent( );
        }

        public PasswordManagerViewModel2 ViewModel
        {
            get { return DataContext as PasswordManagerViewModel2; }
            set { DataContext = value; }
        }

        //private void Button_Click( object sender, RoutedEventArgs e ) {}
    }
}