using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    /// Interaction logic for PasswordGrouping.xaml
    /// </summary>
    public partial class PasswordGrouping
    {
        public PasswordGrouping( )
        {
            InitializeComponent( );
        }

        public PasswordGroupingViewModel ViewModel
        {
            get { return DataContext as PasswordGroupingViewModel; }
            set { DataContext = value; }
        }
    }
}