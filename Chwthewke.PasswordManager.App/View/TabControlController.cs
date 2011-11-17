using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    public class TabControlController
    {
        // TODO unit test me
        public TabControlController( ITabbed tabbed )
        {
            _tabbed = tabbed;
        }

        public ObservableCollection<PasswordEditorViewModel> Editors
        {
            get { return _editors; }
            set
            {
                if ( _editors != null )
                    _editors.CollectionChanged -= OnEditorsChanged;
                _editors = value;
                if ( _editors != null )
                    _editors.CollectionChanged += OnEditorsChanged;
                ResetEditors( );
            }
        }

        private void OnEditorsChanged( object sender, NotifyCollectionChangedEventArgs e )
        {
            switch ( e.Action )
            {
                case NotifyCollectionChangedAction.Add:
                    foreach ( object item in e.NewItems )
                        AddEditor( (PasswordEditorViewModel) item );
                    break;
                case NotifyCollectionChangedAction.Remove:
                    foreach ( object item in e.OldItems )
                        RemoveEditor( (PasswordEditorViewModel) item );
                    break;
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:
                case NotifyCollectionChangedAction.Reset:
                    ResetEditors( );
                    break;
                default:
                    throw new ArgumentOutOfRangeException( );
            }
        }

        private void ResetEditors( )
        {
            ClearEditors( );
            foreach ( PasswordEditorViewModel editorViewModel in Editors )
            {
                AddEditor( editorViewModel );
            }
        }

        private void AddEditor( PasswordEditorViewModel editorViewModel )
        {
            TabItem newItem = CreateTabItem( editorViewModel );
            _tabbed.AddItem( newItem );
            _tabbed.SelectedItem = newItem;
        }


        private void RemoveEditor( PasswordEditorViewModel editorViewModel )
        {
            TabItem tabItem = FindTabItem( editorViewModel );

            if ( tabItem != null )
            {
                _tabbed.RemoveItem( tabItem );
                if ( !_tabbed.SelectedIndex.HasValue && _tabbed.ItemCount > 0 )
                    _tabbed.SelectedIndex = 0;
            }
        }


        private void ClearEditors( )
        {
            _tabbed.Clear( );
        }

        private static TabItem CreateTabItem( PasswordEditorViewModel editorViewModel )
        {
            return new TabItem
                       {
                           Header = new PasswordEditorHeader { ViewModel = editorViewModel },
                           Content = new PasswordEditor { ViewModel = editorViewModel }
                       };
        }

        private TabItem FindTabItem( PasswordEditorViewModel editorViewModel )
        {
            Func<TabItem, bool> hasMatchingEditor =
                item => item.Content is PasswordEditor
                        && ( (PasswordEditor) item.Content ).ViewModel == editorViewModel;

            return _tabbed.Items.FirstOrDefault( hasMatchingEditor );
        }


        private readonly ITabbed _tabbed;
        private ObservableCollection<PasswordEditorViewModel> _editors;
    }
}