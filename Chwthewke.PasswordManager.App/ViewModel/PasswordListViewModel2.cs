using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordListViewModel2 : ObservableObject
    {
        public PasswordListViewModel2( IPasswordManagerStorage storage,
                                       PasswordEditorViewModelFactory2 editorFactory,
                                       StoredPasswordViewModel2.Factory entryFactory )
        {
            _storage = storage;
            _editorFactory = editorFactory;
            _entryFactory = entryFactory;
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditorInternal( null ) );
            UpdateList( );
            EnforceAtLeastOneEditor( );
        }

        public ObservableCollection<StoredPasswordViewModel2> VisibleItems
        {
            get { return _visibleItems; }
            private set
            {
                _visibleItems = value;
                RaisePropertyChanged( ( ) => VisibleItems );
            }
        }

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                if ( value == _filterString )
                    return;
                _filterString = value;
                RaisePropertyChanged( ( ) => FilterString );
                UpdateFilteredListView( );
            }
        }

        public ObservableCollection<PasswordEditorViewModel2> Editors
        {
            get { return _editors; }
        }

        public ICommand OpenEditorCommand
        {
            get { return _openEditorCommand; }
        }

        public void OpenNewEditor( StoredPasswordViewModel2 password )
        {
            if ( password != null )
                OpenNewEditorInternal( password.PasswordDocument );
        }

        public void UpdateList( )
        {
//            _passwordRepository.Reload( );

            _items = new ObservableCollection<StoredPasswordViewModel2>(
                from password in _storage.PasswordRepository.LoadPasswords( )
                orderby password.Key
                select _entryFactory.Invoke( password )
                );

            foreach ( PasswordEditorViewModel2 editor in Editors )
                editor.UpdateFromDatabase( );

            UpdateFilteredListView( );
        }

        private void UpdateFilteredListView( )
        {
            VisibleItems = new ObservableCollection<StoredPasswordViewModel2>( _items.Where( IsItemVisible ) );
        }

        private void OpenNewEditorInternal( PasswordDigestDocument passwordDocument )
        {
            UpdateList( );
            // TODO this null check goes down all the way, why ?
            PasswordEditorViewModel2 editor = passwordDocument == null
                                                  ? _editorFactory.NewPasswordEditor( )
                                                  : _editorFactory.PasswordEditorFor( passwordDocument );
            editor.CloseRequested += EditorRequestedClose;
            editor.StoreModified += StoreModified;

            Editors.Add( editor );

            if ( _forcedEditor != null && _forcedEditor.IsPristine )
                CloseEditor( _forcedEditor );
        }

        private void StoreModified( object sender, EventArgs e )
        {
            UpdateList( );
        }


        private void EditorRequestedClose( object sender, EventArgs e )
        {
            PasswordEditorViewModel2 editor = sender as PasswordEditorViewModel2;
            CloseEditor( editor );
        }

        private void CloseEditor( PasswordEditorViewModel2 editor )
        {
            if ( editor != null )
            {
                editor.CloseRequested -= EditorRequestedClose;
                editor.StoreModified -= StoreModified;
                Editors.Remove( editor );
                if ( editor == _forcedEditor )
                    _forcedEditor = null;
            }

            EnforceAtLeastOneEditor( );
        }


        private bool IsItemVisible( StoredPasswordViewModel2 item )
        {
            return string.IsNullOrWhiteSpace( _filterString ) ||
                   item.Name.Contains( _filterString );
        }

        private void EnforceAtLeastOneEditor( )
        {
            if ( _editors.Count == 0 )
                OpenNewEditorInternal( null );
            _forcedEditor = _editors[ 0 ];
        }

        private ObservableCollection<StoredPasswordViewModel2> _visibleItems;
        private ObservableCollection<StoredPasswordViewModel2> _items;
        private string _filterString;

        private readonly ObservableCollection<PasswordEditorViewModel2> _editors =
            new ObservableCollection<PasswordEditorViewModel2>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordManagerStorage _storage;
        private readonly PasswordEditorViewModelFactory2 _editorFactory;
        private readonly StoredPasswordViewModel2.Factory _entryFactory;
        private PasswordEditorViewModel2 _forcedEditor;
    }
}