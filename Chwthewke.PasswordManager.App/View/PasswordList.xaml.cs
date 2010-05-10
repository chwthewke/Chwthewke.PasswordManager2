using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    /// Interaction logic for PasswordList.xaml
    /// </summary>
    public partial class PasswordList
    {
        public PasswordList( )
        {
            InitializeComponent( );
        }

        public PasswordListViewModel ViewModel
        {
            get { return DataContext as PasswordListViewModel; }
            set { DataContext = value; }
        }
    }
}