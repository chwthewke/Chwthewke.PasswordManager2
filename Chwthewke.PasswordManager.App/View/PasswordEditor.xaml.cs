using System;
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
            get { return DataContext as PasswordEditorViewModel; }
            set
            {
                DataContext = value;
                // TODO temp
                if ( ViewModel != null )
                    ViewModel.Tag = this;
            }
        }

        private void PasswordBox_PasswordChanged( object sender, RoutedEventArgs e )
        {
            // TODO temp
            Console.WriteLine( @"Update ViewModel {0:X} with PasswordBox {1:X}, SecureString {3:X} in Editor {2:X}",
                               ViewModel.GetHashCode( ),
                               _masterPassword.GetHashCode( ),
                               GetHashCode( ),
                               _masterPassword.SecurePassword.GetHashCode( ) );
            ViewModel.UpdateMasterPassword( _masterPassword.SecurePassword );
        }
    }
}