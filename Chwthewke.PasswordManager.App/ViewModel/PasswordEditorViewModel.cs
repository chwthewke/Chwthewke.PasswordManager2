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

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public PasswordEditorViewModel( IPasswordEditorModel model,
                                         IClipboardService clipboardService,
                                         IGuidToColorConverter guidToColor )
        {
            _model = model;
            _clipboardService = clipboardService;
            _guidToColor = guidToColor;
            _derivedPasswords = new ObservableCollection<DerivedPasswordViewModel>(
                _model.DerivedPasswords.Select( dp => new DerivedPasswordViewModel( dp, _model ) ) );

            foreach ( DerivedPasswordViewModel passwordSlotViewModel in DerivedPasswords )
                passwordSlotViewModel.PropertyChanged += OnDerivedPasswordPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );
            _closeCommand = new RelayCommand( RaiseCloseRequested );

            _increaseIterationCommand = new RelayCommand( ExecuteIncreaseIteration, CanExecuteIncreaseIteration );
            _decreaseIterationCommand = new RelayCommand( ExecuteDecreaseIteration, CanExecuteDecreaseIteration );

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
                _model.Key = string.IsNullOrWhiteSpace( value ) ? string.Empty : value;
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

        public int Iteration
        {
            get { return _model.Iteration; }
            set
            {
                _model.Iteration = value;
                RaisePropertyChanged( ( ) => Iteration );
                _increaseIterationCommand.RaiseCanExecuteChanged(  );
                _decreaseIterationCommand.RaiseCanExecuteChanged(  );
                Update( );
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

        public ObservableCollection<DerivedPasswordViewModel> DerivedPasswords
        {
            get { return _derivedPasswords; }
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

        public ICommand DecreaseIterationCommand
        {
            get { return _decreaseIterationCommand; }
        }

        public ICommand IncreaseIterationCommand
        {
            get { return _increaseIterationCommand; }
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

        private void OnDerivedPasswordPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName != "IsSelected" )
                return;

            DerivedPasswordViewModel selected = DerivedPasswords.FirstOrDefault( s => s.IsSelected );
            _model.SelectedPassword = selected == null ? null : selected.Model;

            Update( );

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );

            CopyText = DeriveCopyText( );
        }

        private bool CanExecuteIncreaseIteration( )
        {
            return Iteration < int.MaxValue;
        }

        private void ExecuteIncreaseIteration( )
        {
            Iteration += 1;
        }

        private bool CanExecuteDecreaseIteration( )
        {
            return Iteration > 1;
        }

        private void ExecuteDecreaseIteration( )
        {
            Iteration -= 1;
        }

        private bool CanExecuteSave( )
        {
            return _model.CanSave;
        }

        private void ExecuteSave( )
        {
            if ( !CanExecuteSave( ) )
                return;
            _model.Save( );

            UpdateSaved( );
            RaiseStoreModified( );
        }

        private bool CanExecuteDelete( )
        {
            return _model.CanDelete;
        }

        private void ExecuteDelete( )
        {
            if ( !CanExecuteDelete( ) )
                return;

            _model.Delete( );

            UpdateSaved( );
            RaiseStoreModified( );
        }

        private string DeriveDerivedPassword( )
        {
            IDerivedPasswordModel derivedPasswordModel = _model.SelectedPassword;
            if ( derivedPasswordModel == null )
                return string.Empty;
            return derivedPasswordModel.DerivedPassword.Password;
        }

        private bool CanExecuteCopy( )
        {
            return !String.IsNullOrEmpty( _derivedPassword );
        }

        private void ExecuteCopy( )
        {
            if ( !CanExecuteCopy( ) )
                return;
            _clipboardService.CopyToClipboard( _derivedPassword );
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

            IsKeyReadonly = _model.IsKeyReadonly;

            foreach ( var slot in DerivedPasswords )
            {
                slot.Update( );
            }

            RequiredGuidColor = ConvertGuid( _model.ExpectedMasterPasswordId );

            CopyText = DeriveCopyText( );

            _derivedPassword = DeriveDerivedPassword( );

            _saveCommand.RaiseCanExecuteChanged( );
            _copyCommand.RaiseCanExecuteChanged( );
            _deleteCommand.RaiseCanExecuteChanged( );
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

            return _model.IsDirty ? title + "*" : title;
        }

        private string DeriveCopyText( )
        {
            IDerivedPasswordModel derivedPasswordModel = _model.SelectedPassword;
            string qualifier;
            if ( derivedPasswordModel == null )
                qualifier = Resources.CopyPasswordDefaultQualifier;
            else
                qualifier = Resources.ResourceManager.GetString( PasswordGeneratorTranslator.NameKey( derivedPasswordModel.Generator ) );
            return string.Format( Resources.CopyPasswordTemplate, qualifier );
        }


        private readonly IPasswordEditorModel _model;
        private readonly IClipboardService _clipboardService;
        private readonly IGuidToColorConverter _guidToColor;

        private string _title = NewTitle;
        private bool _isKeyReadonly;

        private Color _requiredGuidColor = Colors.Transparent;
        private Color _actualGuidColor = Colors.Transparent;
        private string _masterPasswordHint = string.Empty;

        private readonly ObservableCollection<DerivedPasswordViewModel> _derivedPasswords;

        private string _copyText;

        private string _derivedPassword;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;
        private readonly IUpdatableCommand _increaseIterationCommand;
        private readonly IUpdatableCommand _decreaseIterationCommand;
        private readonly ICommand _closeCommand;

        public const string NewTitle = "(new)";
    }
}