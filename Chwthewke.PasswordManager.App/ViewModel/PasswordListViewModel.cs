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
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditor( null ) );
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

        private void OpenNewEditor( string passwordKey )
        {
            
        }

        private void CloseEditor( PasswordEditorViewModel editor )
        {
            
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