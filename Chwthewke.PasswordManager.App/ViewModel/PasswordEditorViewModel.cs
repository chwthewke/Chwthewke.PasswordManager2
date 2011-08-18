using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Windows.Input;
using System.Windows.Media;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public delegate PasswordEditorViewModel Factory( string password );

        public PasswordEditorViewModel( IPasswordEditorController controller,
                                        IClipboardService clipboardService,
                                        IGuidToColorConverter guidToColor )
        {
            _controller = controller;

            _clipboardService = clipboardService;
            _guidToColor = guidToColor;
            _slots =
                new ObservableCollection<PasswordSlotViewModel>(
                    _controller.Generators.Select( g => new PasswordSlotViewModel( g ) ) );

            foreach ( PasswordSlotViewModel passwordSlotViewModel in Slots )
                passwordSlotViewModel.PropertyChanged += OnSlotPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );
            _closeCommand = new RelayCommand( RaiseCloseRequested );

            Update( );
        }

        public event EventHandler StoreModified;

        public event EventHandler CloseRequested;

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
                    slot.IsSelected &= _controller.IsPasswordLoaded || _canSelectPasswordSlot;
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

        public Color RequiredGuidColor
        {
            get { return _requiredGuidColor; }
            private set
            {
                if ( _requiredGuidColor == value )
                    return;
                _requiredGuidColor = value;
                RaisePropertyChanged( ( ) => RequiredGuidColor );
            }
        }


        public Color ActualGuidColor
        {
            get { return _actualGuidColor; }
            private set
            {
                if ( _actualGuidColor == value )
                    return;
                _actualGuidColor = value;
                RaisePropertyChanged( ( ) => ActualGuidColor );
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

        public ICommand CloseCommand
        {
            get { return _closeCommand; }
        }

        public void UpdateMasterPassword( SecureString masterPassword )
        {
            _controller.MasterPassword = masterPassword;
            ActualGuidColor = ConvertGuid( _controller.MasterPasswordId );
            Update( );
        }

        private Color ConvertGuid( Guid? masterPasswordId )
        {
            return masterPasswordId.HasValue ? _guidToColor.Convert( masterPasswordId.Value ) : Colors.Transparent;
        }

        private void RaiseStoreModified( )
        {
            EventHandler storeModified = StoreModified;
            if ( storeModified != null )
                storeModified( this, EventArgs.Empty );
        }

        private void RaiseCloseRequested( )
        {
            EventHandler closeRequested = CloseRequested;
            if ( closeRequested != null )
                closeRequested( this, EventArgs.Empty );
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
            // TODO event StoreModified once only ?

            RaiseStoreModified( );

            UpdateSaved( );
            RaiseStoreModified( );
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

            UpdateSaved( );
            RaiseStoreModified( );
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

        private void UpdateSaved( )
        {
            Update( );

            ActualGuidColor = ConvertGuid( _controller.MasterPasswordId );
        }

        private void Update( )
        {
            Title = DeriveTitle( );
            CanSelectPasswordSlot = DeriveCanSelectPassword( );
            IsKeyReadonly = DeriveKeyReadonly( );

            foreach ( var slot in Slots )
            {
                slot.Content = _controller.GeneratedPassword( slot.Generator );
                slot.IsSelected = _controller.SelectedGenerator == slot.Generator;
            }

            RequiredGuidColor = ConvertGuid( _controller.ExpectedMasterPasswordId );

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );
            _deleteCommand.RaiseCanExecuteChanged( );
        }

        private bool DeriveKeyReadonly( )
        {
            return _controller.IsPasswordLoaded;
        }

        private bool DeriveCanSelectPassword( )
        {
            return !_controller.Generators.All( g => string.IsNullOrEmpty( _controller.GeneratedPassword( g ) ) );
        }

        private string DeriveTitle( )
        {
            string title;
            if ( string.IsNullOrWhiteSpace( Key ) )
                title = NewTitle;
            else if ( Key.Length > 25 )
                title = Key.Substring( 0, 24 ) + "...";
            else
                title = Key;

            return _controller.IsDirty ? title + "*" : title;
        }


        private readonly IPasswordEditorController _controller;
        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColor;

        private string _title = NewTitle;
        private bool _canSelectPasswordSlot;
        private bool _isKeyReadonly;

        private Color _requiredGuidColor = Colors.Transparent;
        private Color _actualGuidColor = Colors.Transparent;

        private readonly ObservableCollection<PasswordSlotViewModel> _slots;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;
        private readonly ICommand _closeCommand;

        public const string NewTitle = "(new)";

        public void LoadPasswordForKey( )
        {
            throw new NotImplementedException( );
        }
    }
}