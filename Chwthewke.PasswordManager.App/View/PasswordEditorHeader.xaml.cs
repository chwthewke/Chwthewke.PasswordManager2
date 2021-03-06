﻿using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    /// <summary>
    ///   Interaction logic for PasswordEditorHeader.xaml
    /// </summary>
    public partial class PasswordEditorHeader
    {
        public PasswordEditorHeader( )
        {
            InitializeComponent( );
        }

        public PasswordEditorViewModel ViewModel
        {
            get { return DataContext as PasswordEditorViewModel; }
            set { DataContext = value; }
        }
    }
}