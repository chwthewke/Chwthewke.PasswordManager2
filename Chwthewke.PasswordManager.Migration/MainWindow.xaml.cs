namespace Chwthewke.PasswordManager.Migration
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow( ) : this( null ) {}

        public MainWindow( MainViewModel viewModel )
        {
            if ( viewModel != null )
                ViewModel = viewModel;
            InitializeComponent( );
        }

        public MainViewModel ViewModel
        {
            get { return DataContext as MainViewModel; }
            set { DataContext = value; }
        }
    }
}