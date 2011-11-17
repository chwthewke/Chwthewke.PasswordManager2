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

        public PasswordListViewModel ViewModel
        {
            get { return DataContext as PasswordListViewModel; }
            set { DataContext = value; }
        }

        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );
            if ( e.Property == DataContextProperty && e.NewValue is PasswordListViewModel )
                _tabsController.Editors = ( (PasswordListViewModel) e.NewValue ).Editors;
        }

        private void PasswordItemDoubleClicked( object sender, MouseButtonEventArgs e )
        {
            ListViewItem item = sender as ListViewItem;
            if ( item == null )
                return;

            PasswordListEntryViewModel passwordListEntry = item.DataContext as PasswordListEntryViewModel;

            if ( passwordListEntry != null )
                ViewModel.OpenNewEditor( passwordListEntry );
        }

        private readonly TabControlController _tabsController;
    }
}