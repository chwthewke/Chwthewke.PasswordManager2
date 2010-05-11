namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    /// Interaction logic for PasswordListItem.xaml
    /// </summary>
    public partial class PasswordListItem
    {
        public PasswordListItem( )
        {
            InitializeComponent( );
        }

        public PasswordListItem ViewModel
        {
            get { return DataContext as PasswordListItem; }
            set { DataContext = value; }
        }
    }
}