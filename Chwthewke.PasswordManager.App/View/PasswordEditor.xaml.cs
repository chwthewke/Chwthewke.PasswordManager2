using System.Windows;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    ///   Interaction logic for PasswordEditor.xaml
    /// </summary>
    public partial class PasswordEditor
    {
        public PasswordEditor( )
        {
            InitializeComponent( );
        }

        public PasswordEditorViewModel2 ViewModel
        {
            get { return DataContext as PasswordEditorViewModel2; }
            set { DataContext = value; }
        }

        private void PasswordBoxPasswordChanged( object sender, RoutedEventArgs e )
        {
            ViewModel.UpdateMasterPassword( _masterPassword.SecurePassword );
        }
    }
}