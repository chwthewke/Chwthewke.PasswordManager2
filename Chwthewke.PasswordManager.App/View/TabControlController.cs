using System;
using System.Collections.Specialized;
using System.Linq;
using System.Windows.Controls;
using Chwthewke.PasswordManager.App.ViewModel;

namespace Chwthewke.PasswordManager.App.View
{
    public class TabControlController
    {
        // TODO depend on an interface rather than TabControl
        public TabControlController( TabControl tabControl )
        {
            _tabControl = tabControl;
        }

        public PasswordListViewModel Model
        {
            get { return _model; }
            set
            {
                if ( _model != null )
                    _model.Editors.CollectionChanged -= OnEditorsChanged;
                _model = value;
                if ( _model != null )
                    _model.Editors.CollectionChanged += OnEditorsChanged;
            }
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
                    foreach ( PasswordEditorViewModel editorViewModel in Model.Editors )
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
            TabItem newItem = new TabItem
                                  {
                                      Header = new PasswordEditorHeader { ViewModel = editorViewModel },
                                      Content = new PasswordEditor { ViewModel = editorViewModel }
                                  };
            _tabControl.Items.Add( newItem );
            _tabControl.SelectedItem = newItem;
        }

        private void RemoveEditor( PasswordEditorViewModel editorViewModel )
        {
            TabItem matching = _tabControl.Items
                .OfType<TabItem>( )
                .FirstOrDefault( item => item.Content is PasswordEditor
                                         && ( ( PasswordEditor ) item.Content ).ViewModel == editorViewModel );
            if ( matching != null )
            {
                _tabControl.Items.Remove( matching );
                if ( !_tabControl.Items.IsEmpty && _tabControl.SelectedItem == null )
                    _tabControl.SelectedItem = _tabControl.Items[ 0 ];
            }
        }

        private void ClearEditors( )
        {
            _tabControl.Items.Clear( );
        }


        private PasswordListViewModel _model;
        private readonly TabControl _tabControl;
    }
}