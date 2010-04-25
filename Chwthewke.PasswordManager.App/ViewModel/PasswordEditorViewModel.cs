using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using System.Linq;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public PasswordEditorViewModel( IPasswordEditor editor,
                                        IPasswordStore passwordStore,
                                        IClipboardService clipboardService )
        {
            _editor = editor;
            _passwordStore = passwordStore;
            _clipboardService = clipboardService;
            _slots = new ObservableCollection<PasswordSlotViewModel>(
                _editor.PasswordSlots.Select( g => new PasswordSlotViewModel( g ) ) );
            foreach ( PasswordSlotViewModel passwordSlotViewModel in Slots )
                passwordSlotViewModel.PropertyChanged += OnSlotPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );
            _loadCommand = new RelayCommand( ExecuteLoad );
        }

        public string Key
        {
            get { return _key; }
            set
            {
                if ( _key == value || IsPasswordLoaded )
                    return;
                _key = value;
                OnKeyChanged( false );
            }
        }

        public string Title
        {
            get { return _title; }
            private set
            {
                if ( _title == value )
                    return;
                _title = value;
                RaisePropertyChanged( ( ) => Title );
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                if ( _note == value )
                    return;
                _note = value;
                RaisePropertyChanged( ( ) => Note );
            }
        }

        public bool CanSelectPasswordSlot
        {
            get { return _canSelectPasswordSlot; }
            private set
            {
                if ( _canSelectPasswordSlot == value )
                    return;
                _canSelectPasswordSlot = value;
                RaisePropertyChanged( ( ) => CanSelectPasswordSlot );
                foreach ( PasswordSlotViewModel slot in Slots )
                    slot.IsSelected &= _canSelectPasswordSlot;
            }
        }

        public bool LoadEnabled
        {
            get { return _loadEnabled; }
            private set
            {
                if ( _loadEnabled == value )
                    return;
                _loadEnabled = value;
                RaisePropertyChanged( ( ) => LoadEnabled );
            }
        }

        public bool IsPasswordLoaded
        {
            get { return _isPasswordLoaded; }
            set
            {
                if ( _isPasswordLoaded == value )
                    return;
                _isPasswordLoaded = value;
                RaisePropertyChanged( ( ) => IsPasswordLoaded );
            }
        }


        public ObservableCollection<PasswordSlotViewModel> Slots
        {
            get { return _slots; }
        }

        public ICommand SaveCommand
        {
            get { return _saveCommand; }
        }

        public ICommand DeleteCommand
        {
            get { return _deleteCommand; }
        }

        public ICommand CopyCommand
        {
            get { return _copyCommand; }
        }

        public ICommand LoadCommand
        {
            get { return _loadCommand; }
        }


        public void UpdateMasterPassword( SecureString masterPassword )
        {
            _masterPassword = masterPassword;
            UpdateGeneratedPasswords( );
        }

        private void LoadPasswordDigest( string key )
        {
            //
            // THIS BLOCK : not exactly pristine.
            // IDEA : Add a nullable "backing document/digest" to this class, use it to determine key writability
            _editor.LoadFromStore( key );
            _key = _editor.Key;
            OnKeyChanged( true );
            //
            Note = _editor.Note;
            PasswordSlotViewModel slot = Slots.FirstOrDefault( s => s.Generator.Id == _editor.SavedSlot.Id );
            if ( slot != null )
                slot.IsSelected = true;
        }

        private void OnKeyChanged( bool makeReadonly )
        {
            RaisePropertyChanged( ( ) => Key );
            IsPasswordLoaded = makeReadonly;
            string titleSuffix = IsPasswordLoaded ? "" : "*";
            Title = IsKeyValid ? Key + titleSuffix : NewTitle;
            LoadEnabled = !IsPasswordLoaded && _passwordStore.FindPasswordInfo( Key ) != null;
            UpdateGeneratedPasswords( );
        }

        private void UpdateGeneratedPasswords( )
        {
            CanSelectPasswordSlot = IsKeyValid && !IsPasswordLoaded && _masterPassword != null && _masterPassword.Length > 0;

            if ( CanSelectPasswordSlot )
                UpdateSlots( slot => slot.Generator.MakePassword( Key, _masterPassword ) );
            else
                UpdateSlots( slot => string.Empty );
        }


        private void UpdateSlots( Func<PasswordSlotViewModel, string> slotContent )
        {
            foreach ( PasswordSlotViewModel slot in _slots )
                slot.Content = slotContent( slot );
        }

        private bool IsKeyValid
        {
            get { return !string.IsNullOrWhiteSpace( Key ); }
        }

        private void OnSlotPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName == "IsSelected" )
            {
                _saveCommand.RaiseCanExecuteChanged( );
                _copyCommand.RaiseCanExecuteChanged( );
            }
        }

        private bool HasPassword
        {
            get { return Slots.Any( s => s.IsSelected ); }
        }

        private PasswordSlotViewModel SelectedSlot
        {
            get { return Slots.First( s => s.IsSelected ); }
        }

        private bool CanExecuteSave( )
        {
            return HasPassword;
        }

        private void ExecuteSave( )
        {
            if ( !CanExecuteSave( ) )
                return;
            _editor.Key = Key;
            _editor.Note = Note;
            _editor.GeneratePasswords( _masterPassword );
            _editor.SavedSlot = SelectedSlot.Generator;
        }

        private bool CanExecuteDelete( )
        {
            return IsPasswordLoaded;
        }

        private void ExecuteDelete( )
        {
            if ( !CanExecuteDelete( ) )
                return;
            _editor.Key = Key;
            _editor.SavedSlot = null;
            _editor.Reset( );

            IsPasswordLoaded = false;
            Key = string.Empty;
            Note = string.Empty;
            foreach ( PasswordSlotViewModel slot in Slots )
            {
                slot.IsSelected = false;
            }
        }

        private bool CanExecuteCopy( )
        {
            return HasPassword;
        }

        private void ExecuteCopy( )
        {
            if ( !CanExecuteCopy( ) )
                return;
            _clipboardService.CopyToClipboard( SelectedSlot.Content );
        }

        private void ExecuteLoad( )
        {
            if ( !LoadEnabled )
                return;
            LoadPasswordDigest( Key );
        }

        private readonly IPasswordEditor _editor;
        private readonly IPasswordStore _passwordStore;
        private readonly IClipboardService _clipboardService;

        private string _key = string.Empty;
        private string _title = NewTitle;
        private string _note = string.Empty;
        private SecureString _masterPassword;
        private bool _canSelectPasswordSlot;
        private bool _loadEnabled;
        private bool _isPasswordLoaded;

        private readonly ObservableCollection<PasswordSlotViewModel> _slots;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;
        private readonly IUpdatableCommand _loadCommand;

        public const string NewTitle = "(new)";
    }
}