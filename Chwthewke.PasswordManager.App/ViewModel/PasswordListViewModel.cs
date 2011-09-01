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
                                      StoredPasswordViewModel.Factory storedPasswordViewModelFactory )
        {
            _passwordDatabase = passwordDatabase;
            _editorFactory = editorFactory;
            _storedPasswordViewModelFactory = storedPasswordViewModelFactory;
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

        public string FilterString
        {
            get { return _filterString; }
            set
            {
                if ( value == _filterString )
                    return;
                _filterString = value;
                RaisePropertyChanged( () => FilterString );
                UpdateList( );
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
            Items = new ObservableCollection<StoredPasswordViewModel>(
                from password in _passwordDatabase.Passwords
                where FilterPassword( password )
                orderby password.Key
                select _storedPasswordViewModelFactory.Invoke( password )
                );

            foreach ( PasswordEditorViewModel editor in Editors )
                editor.UpdateFromStore( );
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

        private bool FilterPassword( PasswordDigest digest )
        {
            return string.IsNullOrWhiteSpace( _filterString ) ||
                   digest.Key.Contains( _filterString );
        }

        private ObservableCollection<StoredPasswordViewModel> _items;
        private string _filterString;

        private readonly ObservableCollection<PasswordEditorViewModel> _editors =
            new ObservableCollection<PasswordEditorViewModel>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordDatabase _passwordDatabase;
        private readonly PasswordEditorViewModelFactory _editorFactory;
        private readonly StoredPasswordViewModel.Factory _storedPasswordViewModelFactory;
    }
}