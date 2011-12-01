using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using System.Windows.Media;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordListViewModel : ObservableObject
    {
        public PasswordListViewModel( IPasswordManagerStorage storage,
                                      PasswordEditorViewModelFactory editorFactory,
                                      PasswordListEntryViewModel.Factory entryFactory )
        {
            _storage = storage;
            _editorFactory = editorFactory;
            _entryFactory = entryFactory;
            _openEditorCommand = new RelayCommand( ( ) => OpenNewEditorInternal( null ) );
            UpdateList( );
            EnforceAtLeastOneEditor( );
        }

        public ObservableCollection<PasswordListEntryViewModel> VisibleItems
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

        public void OpenNewEditor( PasswordListEntryViewModel passwordListEntry )
        {
            if ( passwordListEntry != null )
                OpenNewEditorInternal( passwordListEntry.PasswordDocument );
        }

        public void UpdateList( )
        {
            _items = new ObservableCollection<PasswordListEntryViewModel>(
                from password in _storage.PasswordRepository.LoadPasswords( )
                orderby password.Key
                select _entryFactory.Invoke( password )
                );

            foreach ( PasswordEditorViewModel editor in Editors )
                editor.Reload( );

            UpdateFilteredListView( );
        }

        private void UpdateFilteredListView( )
        {
            VisibleItems = new ObservableCollection<PasswordListEntryViewModel>( _items.Where( IsItemVisible ) );
        }

        private void OpenNewEditorInternal( PasswordDigestDocument passwordDocument )
        {
            UpdateList( );

            PasswordEditorViewModel editor = passwordDocument == null
                                                 ? _editorFactory.NewPasswordEditor( )
                                                 : _editorFactory.PasswordEditorFor( passwordDocument );
            editor.CloseRequested += EditorRequestedClose;
            editor.StoreModified += StoreModified;

            Editors.Add( editor );

            CloseEditors( e => e == _forcedEditor && e.IsPristine );
        }

        private void StoreModified( object sender, EventArgs e )
        {
            UpdateList( );
        }


        internal void EditorRequestedClose( object sender, CloseEditorEventArgs e )
        {
            var editor = sender as PasswordEditorViewModel;
            Func<PasswordEditorViewModel, bool> predicate = CloseRequestPredicate( editor, e.Type );
            CloseEditors( predicate );
        }

        private Func<PasswordEditorViewModel, bool> CloseRequestPredicate( PasswordEditorViewModel source, CloseEditorEventType type )
        {
            switch ( type )
            {
                case CloseEditorEventType.Self:
                    return e => e == source;
                case CloseEditorEventType.All:
                    return e => true;
                case CloseEditorEventType.AllButSelf:
                    return e => e != source;
                case CloseEditorEventType.RightOfSelf:
                    return e => Editors.IndexOf( e ) > Editors.IndexOf( source );
                case CloseEditorEventType.Insecure:
                    return e => e.RequiredGuidColor == e.ActualGuidColor && e.RequiredGuidColor != Colors.Transparent;
                default:
                    throw new ArgumentOutOfRangeException( "type" );
            }
        }

        private void CloseEditors( Func<PasswordEditorViewModel, bool> predicate )
        {
            var editorsToClose = Editors.Where( predicate ).ToList( );

            foreach ( var editor in editorsToClose )
            {
                editor.CloseRequested -= EditorRequestedClose;
                editor.StoreModified -= StoreModified;
                Editors.Remove( editor );
                if ( editor == _forcedEditor )
                    _forcedEditor = null;
            }

            EnforceAtLeastOneEditor( );
        }


        private bool IsItemVisible( PasswordListEntryViewModel item )
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

        private ObservableCollection<PasswordListEntryViewModel> _visibleItems;
        private ObservableCollection<PasswordListEntryViewModel> _items;
        private string _filterString;

        private readonly ObservableCollection<PasswordEditorViewModel> _editors =
            new ObservableCollection<PasswordEditorViewModel>( );

        private readonly ICommand _openEditorCommand;
        private readonly IPasswordManagerStorage _storage;
        private readonly PasswordEditorViewModelFactory _editorFactory;
        private readonly PasswordListEntryViewModel.Factory _entryFactory;
        private PasswordEditorViewModel _forcedEditor;
    }
}