using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    ///   Interaction logic for PasswordListItem.xaml
    /// </summary>
    public partial class PasswordListItem
    {
        public PasswordListItem( )
        {
            InitializeComponent( );
        }

        public PasswordListEntryViewModel ViewModel
        {
            get { return DataContext as PasswordListEntryViewModel; }
            set { DataContext = value; }
        }
    }
}