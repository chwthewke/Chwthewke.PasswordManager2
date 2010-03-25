using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;

namespace Chwthewke.PasswordManager.Migration
{
    /// <summary>
    /// Interaction logic for ImporterPanel.xaml
    /// </summary>
    public partial class ImporterPanel
    {
        public ImporterPanel( ) : this( null ) {}

        public ImporterPanel( ImporterViewModel viewModel )
        {
            if ( viewModel != null )
                ViewModel = viewModel;
            InitializeComponent( );
        }

        public ImporterViewModel ViewModel
        {
            get { return DataContext as ImporterViewModel; }
            set { DataContext = value; }
        }

        private void PasswordChanged( object sender, RoutedEventArgs e )
        {
            BindingExpression be = _import.GetBindingExpression( ButtonBase.CommandParameterProperty );
            if ( be != null )
                be.UpdateTarget( );
        }
    }
}