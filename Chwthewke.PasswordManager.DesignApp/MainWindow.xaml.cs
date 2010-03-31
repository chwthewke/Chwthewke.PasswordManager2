using System.Windows;

namespace Chwthewke.PasswordManager.DesignApp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public MainWindow( ) : this( null ) {}

        public MainWindow( DesignViewModel viewModel )
        {
            if ( viewModel != null )
                ViewModel = viewModel;
            InitializeComponent( );
        }

        public DesignViewModel ViewModel
        {
            get { return DataContext as DesignViewModel; }
            set { DataContext = value; }
        }
    }
}