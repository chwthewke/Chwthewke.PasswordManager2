using System.Windows;

namespace Chwthewke.PasswordManager.Migration
{
    /// <summary>
    /// Interaction logic for ImporterPanel.xaml
    /// </summary>
    public partial class ImporterPanel
    {
        public ImporterPanel( ) : this( null ) {}

        public ImporterPanel( ImporterViewModel viewModel )
        {
            if ( viewModel != null )
                ViewModel = viewModel;
            InitializeComponent( );
        }

        public ImporterViewModel ViewModel
        {
            get { return DataContext as ImporterViewModel; }
            set { DataContext = value; }
        }

        private void PasswordChanged( object sender, RoutedEventArgs e )
        {
            ViewModel.UpdateMasterPassword( _password.SecurePassword );
        }
    }
}