using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordListViewModel : ObservableObject
    {
        public PasswordListViewModel( IPasswordDatabase passwordDatabase,
                                      PasswordEditorViewModelFactory editorFactory,
                                      StoredPasswordViewModel.Factory storedPasswordViewModelFactory )
        {
            _passwordDatabase = passwordDatabase;
            _editorFactory = editorFactory;
            _storedPasswordViewModelFactory = storedPasswordViewModelFactory;
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditorInternal( string.Empty ) );
            UpdateList( );
            EnforceAtLeastOneEditor( );
        }

        public ObservableCollection<StoredPasswordViewModel> VisibleItems
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

        public ObservableCollection<PasswordEditorViewModel> Editors
        {
            get { return _editors; }
        }

        public ICommand OpenEditorCommand
        {
            get { return _openEditorCommand; }
        }

        public void OpenNewEditor( StoredPasswordViewModel password )
        {
            if ( password != null && !string.IsNullOrEmpty( password.Name ) )
                OpenNewEditorInternal( password.Name );
        }

        public void UpdateList( )
        {
            _items = new ObservableCollection<StoredPasswordViewModel>(
                from password in _passwordDatabase.Passwords
                orderby password.Key
                select _storedPasswordViewModelFactory.Invoke( password )
                );

            foreach ( PasswordEditorViewModel editor in Editors )
                editor.UpdateFromStore( );

            UpdateFilteredListView( );
        }

        private void UpdateFilteredListView( )
        {
            VisibleItems = new ObservableCollection<StoredPasswordViewModel>( _items.Where( IsItemVisible ) );
        }

        private void OpenNewEditorInternal( string passwordKey )
        {
            PasswordEditorViewModel editor = _editorFactory.PasswordEditorFor( passwordKey );
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
            PasswordEditorViewModel editor = sender as PasswordEditorViewModel;
            CloseEditor( editor );
        }

        private void CloseEditor( PasswordEditorViewModel editor )
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


        private bool IsItemVisible( StoredPasswordViewModel item )
        {
            return string.IsNullOrWhiteSpace( _filterString ) ||
                   item.Name.Contains( _filterString );
        }

        private void EnforceAtLeastOneEditor( )
        {
            if ( _editors.Count == 0 )
                OpenNewEditorInternal( string.Empty );
            _forcedEditor = _editors[ 0 ];
        }

        private ObservableCollection<StoredPasswordViewModel> _visibleItems;
        private IList<StoredPasswordViewModel> _items;
        private string _filterString;

        private readonly ObservableCollection<PasswordEditorViewModel> _editors =
            new ObservableCollection<PasswordEditorViewModel>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordDatabase _passwordDatabase;
        private readonly PasswordEditorViewModelFactory _editorFactory;
        private readonly StoredPasswordViewModel.Factory _storedPasswordViewModelFactory;
        private PasswordEditorViewModel _forcedEditor;
    }
}