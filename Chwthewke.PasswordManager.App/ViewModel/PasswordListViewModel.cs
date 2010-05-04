using System;
using System.Collections.ObjectModel;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordListViewModel : ObservableObject
    {
        public PasswordListViewModel( IPasswordStore store, IPasswordEditorFactory editorFactory )
        {
            _store = store;
            _editorFactory = editorFactory;
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditorInternal( null ) );
            UpdatePasswords( );
        }

        public ObservableCollection<PasswordListItem> Items
        {
            get { return _items; }
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
            throw new NotImplementedException( );
        }

        private void EditorRequestedClose( object sender, EventArgs e )
        {
            throw new NotImplementedException( );
        }

        private void CloseEditor( PasswordEditorViewModel editor )
        {
            editor.CloseRequested -= EditorRequestedClose;
            editor.StoreModified -= StoreModified;
            Editors.Remove( editor );
        }
        
        private void UpdatePasswords( )
        {
            throw new NotImplementedException( );
        }


        private readonly ObservableCollection<PasswordListItem> _items = 
            new ObservableCollection<PasswordListItem>( );
        private readonly ObservableCollection<PasswordEditorViewModel> _editors = 
            new ObservableCollection<PasswordEditorViewModel>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordStore _store;
        private readonly IPasswordEditorFactory _editorFactory;
    }
}