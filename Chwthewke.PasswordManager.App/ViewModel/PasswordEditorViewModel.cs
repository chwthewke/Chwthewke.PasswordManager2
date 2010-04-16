using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Security;
using System.Windows.Input;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.Editor;
using System.Linq;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public PasswordEditorViewModel( IPasswordEditor editor )
        {
            _editor = editor;
            _slots = new ObservableCollection<PasswordSlotViewModel>(
                _editor.PasswordSlots.Select( g => new PasswordSlotViewModel( g ) ) );
            foreach ( PasswordSlotViewModel passwordSlotViewModel in Slots )
                passwordSlotViewModel.PropertyChanged += OnSlotPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );
        }

        public string Key
        {
            get { return _key; }
            set
            {
                if ( _key == value )
                    return;
                _key = value;
                RaisePropertyChanged( ( ) => Key );
                OnKeyChanged( );
            }
        }

        public string Title
        {
            get { return _title; }
            set
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

        public void UpdateMasterPassword( SecureString masterPassword )
        {
            _masterPassword = masterPassword;
            UpdateGeneratedPasswords( );
        }

        private void OnKeyChanged( )
        {
            Title = IsKeyValid ? Key + "*" : NewTitle;
            UpdateGeneratedPasswords( );
        }

        private void UpdateGeneratedPasswords( )
        {
            CanSelectPasswordSlot = IsKeyValid && _masterPassword != null && _masterPassword.Length > 0;

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
                CommandManager.InvalidateRequerySuggested( );
        }

        private bool HasPassword( )
        {
            return Slots.Any( s => s.IsSelected );
        }

        private bool CanExecuteSave( )
        {
            return HasPassword( );
        }

        private void ExecuteSave( ) {}

        private bool CanExecuteDelete( )
        {
            return false;
        }

        private void ExecuteDelete( ) {}

        private bool CanExecuteCopy( )
        {
            return HasPassword(  );
        }

        private void ExecuteCopy( ) {}

        private readonly IPasswordEditor _editor;

        private string _key = string.Empty;
        private string _title = NewTitle;
        private string _note = string.Empty;
        private SecureString _masterPassword;
        private bool _canSelectPasswordSlot;

        private readonly ObservableCollection<PasswordSlotViewModel> _slots;

        private readonly ICommand _saveCommand;
        private readonly ICommand _deleteCommand;
        private readonly ICommand _copyCommand;

        public const string NewTitle = "(new)";
    }
}