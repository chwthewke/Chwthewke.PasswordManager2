using System;
using System.Windows.Controls;
using System.Windows.Input;
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

        private void PasswordItemDoubleClicked( object sender, MouseButtonEventArgs e )
        {
            ListViewItem item = sender as ListViewItem;
            if (item == null)
                return;

            StoredPasswordViewModel storedPassword = item.DataContext as StoredPasswordViewModel;

            if ( storedPassword != null )
                ViewModel.OpenNewEditor( storedPassword );
        }
    }
}