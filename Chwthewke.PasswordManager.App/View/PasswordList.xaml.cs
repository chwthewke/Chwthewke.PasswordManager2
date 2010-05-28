using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows;
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

        protected override void OnPropertyChanged( DependencyPropertyChangedEventArgs e )
        {
            base.OnPropertyChanged( e );
            if ( e.Property == DataContextProperty )
            {
                DetachViewModelListeners( e.OldValue as PasswordListViewModel );
                AttachViewModelListeners( e.NewValue as PasswordListViewModel );
            }
        }

        private void AttachViewModelListeners( PasswordListViewModel viewModel )
        {
            if ( viewModel != null )
                viewModel.Editors.CollectionChanged += OnEditorsChanged;
        }

        private void DetachViewModelListeners( PasswordListViewModel viewModel )
        {
            if ( viewModel != null )
                viewModel.Editors.CollectionChanged -= OnEditorsChanged;
        }

        private void OnEditorsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    foreach ( object item in e.NewItems )
                        AddEditor( ( PasswordEditorViewModel ) item );
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach ( object item in e.OldItems )
                        RemoveEditor( ( PasswordEditorViewModel ) item );
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    ClearEditors( );
                    foreach ( PasswordEditorViewModel editorViewModel in ViewModel.Editors )
                    {
                        AddEditor( editorViewModel );
                    }
                    break;
                default:
                    throw new ArgumentOutOfRangeException( );
            }
        }

        private void AddEditor( PasswordEditorViewModel editorViewModel )
        {
            _editorTabs.Items.Add( new TabItem
                                       {
                                           Header = new PasswordEditorHeader { ViewModel = editorViewModel },
                                           Content = new PasswordEditor { ViewModel = editorViewModel }
                                       } );
        }

        private void RemoveEditor( PasswordEditorViewModel editorViewModel )
        {
            TabItem matching = _editorTabs.Items
                .OfType<TabItem>( )
                .FirstOrDefault( item => item.Content is PasswordEditor
                                         && ( ( PasswordEditor ) item.Content ).ViewModel == editorViewModel );
            if ( matching != null )
                _editorTabs.Items.Remove( matching );
        }

        private void ClearEditors( )
        {
            _editorTabs.Items.Clear( );
        }

        private void PasswordItemDoubleClicked( object sender, MouseButtonEventArgs e )
        {
            ListViewItem item = sender as ListViewItem;
            if ( item == null )
                return;

            StoredPasswordViewModel storedPassword = item.DataContext as StoredPasswordViewModel;

            if ( storedPassword != null )
                ViewModel.OpenNewEditor( storedPassword );
        }
    }
}