using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using System.Linq;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public PasswordEditorViewModel( IPasswordEditorController controller,
                                        IClipboardService clipboardService )
        {
            _controller = controller;

            _clipboardService = clipboardService;
            _slots = new ObservableCollection<PasswordSlotViewModel>(
                _controller.Generators.Select( g => new PasswordSlotViewModel( g ) ) );

            foreach ( PasswordSlotViewModel passwordSlotViewModel in Slots )
                passwordSlotViewModel.PropertyChanged += OnSlotPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );
            _loadCommand = new RelayCommand( ExecuteLoad );
        }

        public string Key
        {
            get { return _controller.Key; }
            set
            {
                if ( IsKeyReadonly )
                    return;
                _controller.Key = value;
                RaisePropertyChanged( ( ) => Key );
                Update( );
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
            get { return _controller.Note; }
            set
            {
                _controller.Note = value;
                RaisePropertyChanged( ( ) => Note );
                Update( );
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

        public bool IsLoadEnabled
        {
            get { return _isLoadEnabled; }
            private set
            {
                if ( _isLoadEnabled == value )
                    return;
                _isLoadEnabled = value;
                RaisePropertyChanged( ( ) => IsLoadEnabled );
            }
        }

        public bool IsKeyReadonly
        {
            get { return _isKeyReadonly; }
            private set
            {
                if ( _isKeyReadonly == value )
                    return;
                _isKeyReadonly = value;
                RaisePropertyChanged( ( ) => IsKeyReadonly );
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
            _controller.MasterPassword = masterPassword;
            Update( );
        }

        private void OnSlotPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName != "IsSelected" )
                return;

            PasswordSlotViewModel selectedSlot = Slots.FirstOrDefault( s => s.IsSelected );
            _controller.SelectedGenerator = selectedSlot == null ? null : selectedSlot.Generator;

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );
        }

        private bool HasPassword
        {
            get { return CanSelectPasswordSlot && _controller.SelectedGenerator != null; }
        }

        private bool CanExecuteSave( )
        {
            return HasPassword;
        }

        private void ExecuteSave( )
        {
            if ( !CanExecuteSave( ) )
                return;
            _controller.SavePassword( );
        }

        private bool CanExecuteDelete( )
        {
            return _controller.IsPasswordLoaded;
        }

        private void ExecuteDelete( )
        {
            if ( !CanExecuteDelete( ) )
                return;

            _controller.DeletePassword( );
            Update( );
        }

        private bool CanExecuteCopy( )
        {
            return HasPassword;
        }

        private void ExecuteCopy( )
        {
            if ( !CanExecuteCopy( ) )
                return;
            _clipboardService.CopyToClipboard( _controller.GeneratedPassword( _controller.SelectedGenerator ) );
        }

        private void ExecuteLoad( )
        {
            if ( !IsLoadEnabled )
                return;
            _controller.LoadPassword( );
            Update( );
        }

        private void Update( )
        {
            Title = DeriveTitle( );
            CanSelectPasswordSlot = DeriveCanSelectPassword( );
            IsLoadEnabled = DeriveLoadEnabled( );
            IsKeyReadonly = DeriveKeyReadonly( );

            foreach ( var slot in Slots )
            {
                slot.Content = _controller.GeneratedPassword( slot.Generator );
                slot.IsSelected = _controller.SelectedGenerator == slot.Generator;
            }
        }

        private bool DeriveKeyReadonly( )
        {
            return _controller.IsPasswordLoaded;
        }

        private bool DeriveLoadEnabled( )
        {
            return _controller.IsKeyStored && !_controller.IsPasswordLoaded;
        }

        private bool DeriveCanSelectPassword( )
        {
            return _controller.IsPasswordLoaded ||
                   !_controller.Generators.All( g => string.IsNullOrEmpty( _controller.GeneratedPassword( g ) ) );
        }

        private string DeriveTitle( )
        {
            string title = string.IsNullOrWhiteSpace( Key ) ? NewTitle : Key;

            return _controller.IsDirty ? title + "*" : title;
        }


        private readonly IPasswordEditorController _controller;

        private readonly IClipboardService _clipboardService;

        private string _title = NewTitle;
        private bool _canSelectPasswordSlot;
        private bool _isLoadEnabled;
        private bool _isKeyReadonly;

        private readonly ObservableCollection<PasswordSlotViewModel> _slots;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;
        private readonly IUpdatableCommand _loadCommand;

        public const string NewTitle = "(new)";
    }
}