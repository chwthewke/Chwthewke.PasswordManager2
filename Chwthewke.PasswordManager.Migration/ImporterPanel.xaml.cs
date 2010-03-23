using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace Chwthewke.PasswordManager.Migration
{
    /// <summary>
    /// Interaction logic for ImporterPanel.xaml
    /// </summary>
    public partial class ImporterPanel
    {
        public ImporterPanel( ): this( null )
        {
            
        }

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
    }
}