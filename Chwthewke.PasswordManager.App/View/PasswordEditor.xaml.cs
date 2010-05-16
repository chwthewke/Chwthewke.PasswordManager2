using System.Windows;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    /// Interaction logic for PasswordEditor.xaml
    /// </summary>
    public partial class PasswordEditor
    {
        public PasswordEditor( )
        {
            InitializeComponent( );
        }

        public PasswordEditorViewModel ViewModel
        {
            get { return DataContext as PasswordEditorViewModel;}
            set { DataContext = value; }
        }

        private void PasswordBox_PasswordChanged( object sender, RoutedEventArgs e )
        {
            ViewModel.UpdateMasterPassword( _masterPassword.SecurePassword );
        }
    }
}
