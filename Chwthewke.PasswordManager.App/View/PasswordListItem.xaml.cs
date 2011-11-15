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

        public StoredPasswordViewModel2 ViewModel
        {
            get { return DataContext as StoredPasswordViewModel2; }
            set { DataContext = value; }
        }
    }
}