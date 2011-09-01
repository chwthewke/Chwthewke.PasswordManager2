using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Windows.Input;
using System.Windows.Media;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
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

        public string CopyText
        {
            get { return _copyText; }
            set
            {
                if ( _copyText == value )
                    return;
                _copyText = value;
                RaisePropertyChanged( () => CopyText );
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

        public void UpdateFromStore( )
        {
            _controller.ReloadBaseline( );
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

            CopyText = DeriveCopyText( );
        }

        private bool CanExecuteSave( )
        {
            return _controller.IsSaveable;
        }

        private void ExecuteSave( )
        {
            if ( !CanExecuteSave( ) )
                return;
            _controller.SavePassword( );

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
            return !String.IsNullOrEmpty( _controller.SelectedPassword );
        }

        private void ExecuteCopy( )
        {
            if ( !CanExecuteCopy( ) )
                return;
            _clipboardService.CopyToClipboard( _controller.SelectedPassword );
        }

        private void UpdateSaved( )
        {
            Update( );

            ActualGuidColor = ConvertGuid( _controller.MasterPasswordId );
        }

        private void Update( )
        {
            Title = DeriveTitle( );
            IsKeyReadonly = DeriveKeyReadonly( );

            foreach ( var slot in Slots )
            {
                // move controller reference into slots ?
                slot.Content = _controller.GeneratedPassword( slot.Generator );
                slot.IsSelected = _controller.SelectedGenerator == slot.Generator;
            }

            RequiredGuidColor = ConvertGuid( _controller.ExpectedMasterPasswordId );

            CopyText = DeriveCopyText( );

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );
            _deleteCommand.RaiseCanExecuteChanged( );
        }

        private bool DeriveKeyReadonly( )
        {
            return _controller.IsPasswordLoaded;
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

            return _controller.IsSaveable ? title + "*" : title;
        }

        private string DeriveCopyText( )
        {
            IPasswordGenerator selectedGenerator = _controller.SelectedGenerator;
            string qualifier = selectedGenerator == null
                            ? Resources.ResourceManager.GetString( "CopyPasswordDefaultQualifier" )
                            : Resources.ResourceManager.GetString( PasswordGeneratorTranslator.NameKey( selectedGenerator ) );
            return string.Format(
                Resources.ResourceManager.GetString( "CopyPasswordTemplate" ),
                qualifier );

        }


        private readonly IPasswordEditorController _controller;
        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColor;

        private string _title = NewTitle;
        private bool _isKeyReadonly;

        private Color _requiredGuidColor = Colors.Transparent;
        private Color _actualGuidColor = Colors.Transparent;

        private readonly ObservableCollection<PasswordSlotViewModel> _slots;

        private string _copyText;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;
        private readonly ICommand _closeCommand;

        public const string NewTitle = "(new)";
    }
}