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
    public class PasswordEditorViewModel2 : ObservableObject
    {
        public PasswordEditorViewModel2( IPasswordEditorModel model,
                                         IClipboardService clipboardService,
                                         IGuidToColorConverter guidToColor )
        {
            _model = model;
            _clipboardService = clipboardService;
            _guidToColor = guidToColor;
            _slots = new ObservableCollection<PasswordSlotViewModel2>(
                    _model.DerivedPasswords.Select( dp => new PasswordSlotViewModel2( dp, _model ) ) );

            foreach ( PasswordSlotViewModel2 passwordSlotViewModel in Slots )
                passwordSlotViewModel.PropertyChanged += OnSlotPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );
            _closeCommand = new RelayCommand( RaiseCloseRequested );

            Update( );
        }

        public event EventHandler StoreModified;

        public event EventHandler CloseRequested;

        public bool IsPristine
        {
            get { return Key == string.Empty && Note == string.Empty && _model.MasterPassword.Length == 0; }
        }

        public string Key
        {
            get { return _model.Key; }
            set
            {
                _model.Key = value;
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
            get { return _model.Note; }
            set
            {
                _model.Note = value;
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
                MasterPasswordHint = DerivePasswordHint( );
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
                MasterPasswordHint = DerivePasswordHint( );
            }
        }

        public string MasterPasswordHint
        {
            get { return _masterPasswordHint; }
            set
            {
                if ( value == _masterPasswordHint )
                    return;
                _masterPasswordHint = value;
                RaisePropertyChanged( ( ) => MasterPasswordHint );
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
                RaisePropertyChanged( ( ) => CopyText );
            }
        }

        public ObservableCollection<PasswordSlotViewModel2> Slots
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
            _model.MasterPassword = masterPassword;
            ActualGuidColor = ConvertGuid( _model.MasterPasswordId );
            Update( );
        }

        public void UpdateFromDatabase( )
        {
            _model.Reload( );
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

            PasswordSlotViewModel2 selectedSlot = Slots.FirstOrDefault( s => s.IsSelected );
            _model.SelectedGenerator = selectedSlot == null ? null : selectedSlot.Generator;

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );

            CopyText = DeriveCopyText( );
        }

        private bool CanExecuteSave( )
        {
            return _model.IsSaveable;
        }

        private void ExecuteSave( )
        {
            if ( !CanExecuteSave( ) )
                return;
            _model.SavePassword( );

            UpdateSaved( );
            RaiseStoreModified( );
        }

        private bool CanExecuteDelete( )
        {
            return _model.IsPasswordLoaded;
        }

        private void ExecuteDelete( )
        {
            if ( !CanExecuteDelete( ) )
                return;

            _model.DeletePassword( );

            UpdateSaved( );
            RaiseStoreModified( );
        }

        private bool CanExecuteCopy( )
        {
            return !String.IsNullOrEmpty( _model.SelectedPassword );
        }

        private void ExecuteCopy( )
        {
            if ( !CanExecuteCopy( ) )
                return;
            _clipboardService.CopyToClipboard( _model.SelectedPassword );
        }

        private void UpdateSaved( )
        {
            Update( );

            ActualGuidColor = ConvertGuid( _model.MasterPasswordId );
        }

        private string DerivePasswordHint( )
        {
            if ( _model.ExpectedMasterPasswordId == null ) return string.Empty;

            if ( _model.MasterPasswordId == _model.ExpectedMasterPasswordId )
                return Resources.PasswordHintFulfilled;

            return Resources.PasswordHint;
        }

        private void Update( )
        {
            Title = DeriveTitle( );
            IsKeyReadonly = DeriveKeyReadonly( );

            foreach ( var slot in Slots )
            {
                slot.Update( );
            }

            RequiredGuidColor = ConvertGuid( _model.ExpectedMasterPasswordId );

            CopyText = DeriveCopyText( );

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );
            _deleteCommand.RaiseCanExecuteChanged( );
        }

        private bool DeriveKeyReadonly( )
        {
            return _model.IsPasswordLoaded;
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

            return _model.IsSaveable ? title + "*" : title;
        }

        private string DeriveCopyText( )
        {
            IPasswordGenerator selectedGenerator = _model.SelectedGenerator;
            string qualifier = selectedGenerator == null
                                   ? Resources.CopyPasswordDefaultQualifier
                                   : Resources.ResourceManager.GetString( PasswordGeneratorTranslator.NameKey( selectedGenerator ) );
            return string.Format(
                Resources.CopyPasswordTemplate,
                qualifier );
        }


        private readonly IPasswordEditorModel _model;
        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColor;

        private string _title = NewTitle;
        private bool _isKeyReadonly;

        private Color _requiredGuidColor = Colors.Transparent;
        private Color _actualGuidColor = Colors.Transparent;
        private string _masterPasswordHint = string.Empty;

        private readonly ObservableCollection<PasswordSlotViewModel2> _slots;

        private string _copyText;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;
        private readonly ICommand _closeCommand;

        public const string NewTitle = "(new)";
    }
}