using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    ///   Interaction logic for PasswordList.xaml
    /// </summary>
    public partial class PasswordList
    {
        public PasswordList( )
        {
            InitializeComponent( );
            _tabsController = new TabControlController( new TabControlWrapper( _editorTabs ) );
        }

        public PasswordListViewModel2 ViewModel
        {
            get { return DataContext as PasswordListViewModel2; }
            set { DataContext = value; }
        }

        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );
            if ( e.Property == DataContextProperty && e.NewValue is PasswordListViewModel2 )
                _tabsController.Editors = ( (PasswordListViewModel2) e.NewValue ).Editors;
        }

        private void PasswordItemDoubleClicked( object sender, MouseButtonEventArgs e )
        {
            ListViewItem item = sender as ListViewItem;
            if ( item == null )
                return;

            StoredPasswordViewModel2 storedPassword = item.DataContext as StoredPasswordViewModel2;

            if ( storedPassword != null )
                ViewModel.OpenNewEditor( storedPassword );
        }

        private readonly TabControlController _tabsController;
    }
}