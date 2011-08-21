using System;
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
                                      IGuidToColorConverter guidConverter )
        {
            _passwordDatabase = passwordDatabase;
            _editorFactory = editorFactory;
            _guidConverter = guidConverter;
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditorInternal( string.Empty ) );
            UpdateList( );
        }

        public ObservableCollection<StoredPasswordViewModel> Items
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

        public void OpenNewEditor( StoredPasswordViewModel password )
        {
            if ( password != null && !string.IsNullOrEmpty( password.Name ) )
                OpenNewEditorInternal( password.Name );
        }

        private void OpenNewEditorInternal( string passwordKey )
        {
            PasswordEditorViewModel editor = _editorFactory.PasswordEditorFor( passwordKey );
            editor.CloseRequested += EditorRequestedClose;
            editor.StoreModified += StoreModified;

            Editors.Add( editor );
        }

        private void StoreModified( object sender, EventArgs e )
        {
            UpdateList( );
        }


        public void UpdateList( )
        {
            Items = new ObservableCollection<StoredPasswordViewModel>(
                from password in _passwordDatabase.Passwords
                orderby password.Key
                select new StoredPasswordViewModel
                           {
                               Name = password.Key,
                               MasterPasswordGuid = password.MasterPasswordId,
                               MasterPasswordColor = _guidConverter.Convert( password.MasterPasswordId )
                           }
                );

            foreach ( PasswordEditorViewModel editor in Editors )
                editor.UpdateFromStore( );
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

        private ObservableCollection<StoredPasswordViewModel> _items;

        private readonly ObservableCollection<PasswordEditorViewModel> _editors =
            new ObservableCollection<PasswordEditorViewModel>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordDatabase _passwordDatabase;
        private readonly PasswordEditorViewModelFactory _editorFactory;
        private readonly IGuidToColorConverter _guidConverter;
    }
}