using System;
using System.Collections.ObjectModel;
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
                Title = Key + "*";
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
            if ( string.IsNullOrWhiteSpace( Key ) )
                return;
            foreach ( PasswordSlotViewModel slot in _slots )
                slot.Content = slot.Generator.MakePassword( Key, masterPassword );
        }

        private bool CanExecuteSave( )
        {
            return false;
        }

        private void ExecuteSave( ) {}

        private bool CanExecuteDelete( )
        {
            return false;
        }

        private void ExecuteDelete( ) {}

        private bool CanExecuteCopy( )
        {
            return false;
        }

        private void ExecuteCopy( ) {}

        private readonly IPasswordEditor _editor;

        private string _key = string.Empty;
        private string _title = NewTitle;
        private string _note = string.Empty;

        private readonly ObservableCollection<PasswordSlotViewModel> _slots;


        private readonly ICommand _saveCommand;
        private readonly ICommand _deleteCommand;
        private readonly ICommand _copyCommand;

        public static string NewTitle = "(new)";
    }
}