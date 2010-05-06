using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordListViewModel : ObservableObject
    {
        public PasswordListViewModel( IPasswordStore store, IPasswordEditorFactory editorFactory )
        {
            _store = store;
            _editorFactory = editorFactory;
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditorInternal( null ) );
            UpdateList( );
        }

        public ObservableCollection<PasswordListItem> Items
        {
            get { return _items; }
            private set
            {
                _items = value;
                RaisePropertyChanged( ( ) => Items );
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

        public void OpenNewEditor( string passwordKey )
        {
            if ( !string.IsNullOrEmpty( passwordKey ) )
                OpenNewEditorInternal( passwordKey );
        }

        private void OpenNewEditorInternal( string passwordKey )
        {
            PasswordEditorViewModel editor = _editorFactory.CreatePasswordEditor( );
            if ( passwordKey != null )
            {
                editor.Key = passwordKey;
                editor.LoadCommand.Execute( null );
            }
            editor.CloseRequested += EditorRequestedClose;
            editor.StoreModified += StoreModified;

            Editors.Add( editor );
        }

        private void StoreModified( object sender, EventArgs e )
        {
            UpdateList( );
        }

        private void UpdateList( )
        {
            Items = new ObservableCollection<PasswordListItem>(
                from password in _store.Passwords
                orderby password.Key
                select new PasswordListItem { Name = password.Key, MasterPasswordGuid = password.MasterPasswordId }
                );
        }


        private void EditorRequestedClose( object sender, EventArgs e )
        {
            PasswordEditorViewModel editor = sender as PasswordEditorViewModel;
            if ( editor != null )
                CloseEditor( editor );
        }

        private void CloseEditor( PasswordEditorViewModel editor )
        {
            editor.CloseRequested -= EditorRequestedClose;
            editor.StoreModified -= StoreModified;
            Editors.Remove( editor );
        }

        private ObservableCollection<PasswordListItem> _items;

        private readonly ObservableCollection<PasswordEditorViewModel> _editors =
            new ObservableCollection<PasswordEditorViewModel>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordStore _store;
        private readonly IPasswordEditorFactory _editorFactory;
    }
}