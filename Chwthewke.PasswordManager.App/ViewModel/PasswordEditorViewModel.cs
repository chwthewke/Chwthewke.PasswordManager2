using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Security;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Chwthewke.MvvmUtils;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class PasswordEditorViewModel : ObservableObject
    {
        public PasswordEditorViewModel( IPasswordEditorModel model,
                                        IClipboardService clipboardService,
                                        IDialogService dialogService,
                                        IGuidToColorConverter guidToColor )
        {
            _model = model;
            _clipboardService = clipboardService;
            _dialogService = dialogService;
            _guidToColor = guidToColor;
            _derivedPasswords = new ObservableCollection<DerivedPasswordViewModel>(
                _model.DerivedPasswords.Select( dp => new DerivedPasswordViewModel( dp, _model ) ) );

            foreach ( DerivedPasswordViewModel passwordSlotViewModel in DerivedPasswords )
                passwordSlotViewModel.PropertyChanged += OnDerivedPasswordPropertyChanged;

            _saveCommand = new RelayCommand( ExecuteSave, CanExecuteSave );
            _copyCommand = new RelayCommand( ExecuteCopy, CanExecuteCopy );
            _deleteCommand = new RelayCommand( ExecuteDelete, CanExecuteDelete );

            _closeSelfCommand = new RelayCommand( ( ) => RaiseCloseRequested( CloseEditorEventType.Self ) );
            _closeAllCommand = new RelayCommand( ( ) => RaiseCloseRequested( CloseEditorEventType.All ) );
            _closeAllButSelfCommand = new RelayCommand( ( ) => RaiseCloseRequested( CloseEditorEventType.AllButSelf ) );
            _closeToTheRightCommand = new RelayCommand( ( ) => RaiseCloseRequested( CloseEditorEventType.RightOfSelf ) );
            _closeInsecureCommand = new RelayCommand( ( ) => RaiseCloseRequested( CloseEditorEventType.Insecure ) );

            _scheduler = new ExclusiveDelayedScheduler( );

            Refresh( );
        }

        public delegate PasswordEditorViewModel Factory( IPasswordEditorModel model );

        public event EventHandler StoreModified;

        public event EventHandler<CloseEditorEventArgs> CloseRequested;

        public bool IsPristine
        {
            get
            {
                return Key == string.Empty &&
                       Note == string.Empty &&
                       _model.MasterPassword.Length == 0 &&
                       Iteration == 1 &&
                       DerivedPasswords.All( p => !p.IsSelected );
            }
        }

        public string Key
        {
            get { return _model.Key; }
            set
            {
                _model.Key = string.IsNullOrWhiteSpace( value ) ? string.Empty : value;
                RaisePropertyChanged( ( ) => Key );

                ScheduleDerivedPasswordUpdate( );

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

                ScheduleDerivedPasswordUpdate( );

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
            get { return _model.IsKeyReadonly; }
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

        public ICommand CloseSelfCommand
        {
            get { return _closeSelfCommand; }
        }

        public string CloseSelfText
        {
            get { return Resources.CloseSelf; }
        }

        public ICommand CloseAllCommand
        {
            get { return _closeAllCommand; }
        }

        public string CloseAllText
        {
            get { return Resources.CloseAll; }
        }

        public ICommand CloseAllButSelfCommand
        {
            get { return _closeAllButSelfCommand; }
        }

        public string CloseAllButSelfText
        {
            get { return Resources.CloseAllButSelf; }
        }

        public ICommand CloseToTheRightCommand
        {
            get { return _closeToTheRightCommand; }
        }

        public string CloseToTheRightText
        {
            get { return Resources.CloseAllToTheRight; }
        }

        public ICommand CloseInsecureCommand
        {
            get { return _closeInsecureCommand; }
        }

        public string CloseInsecureText
        {
            get { return Resources.CloseAllInsecure; }
        }

        public void UpdateMasterPassword( SecureString masterPassword )
        {
            _model.MasterPassword = masterPassword;

            ScheduleDerivedPasswordUpdate( );
            Update( );
        }

        public void Reload( )
        {
            _model.Reload( );
            Refresh( );
        }

        private void ScheduleDerivedPasswordUpdate( )
        {
            Dispatcher currentDispatcher = Dispatcher.CurrentDispatcher;
            var actions = new Action[ ]
                                    {
                                        ( ) => _model.UpdateDerivedPasswords( ),
                                        ( ) => currentDispatcher.BeginInvoke( new Action( Update ) )
                                    };

            _scheduler.ScheduleActions( 200, actions );
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

        private void RaiseCloseRequested( CloseEditorEventType closeEditorEventType )
        {
            EventHandler<CloseEditorEventArgs> closeRequested = CloseRequested;
            if ( closeRequested != null )
                closeRequested( this, new CloseEditorEventArgs( closeEditorEventType ) );
        }

        private void OnDerivedPasswordPropertyChanged( object sender, PropertyChangedEventArgs e )
        {
            if ( e.PropertyName != "IsSelected" )
                return;

            var source = sender as DerivedPasswordViewModel;
            if ( source == null || !source.IsSelected )
                return;

            _model.SelectedPassword = source.Model;

            Update( );
        }

        private bool CanExecuteSave( )
        {
            return _model.CanSave;
        }

        private void ExecuteSave( )
        {
            if ( !CanExecuteSave( ) )
                return;

            try
            {
                _model.Save( );
            }
            catch ( PasswordsFileException e )
            {
                _dialogService.ShowFileError( string.Format( "Error while saving password: {0}", e.Message ) );
            }

            Refresh( );
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

            try
            {
                _model.Delete( );
            }
            catch ( PasswordsFileException e )
            {
                _dialogService.ShowFileError( string.Format( "Error while deleting password: {0}", e.Message ) );
            }

            Refresh( );
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

            ActualGuidColor = ConvertGuid( _model.MasterPasswordId );
            MasterPasswordHint = DerivePasswordHint( );

            foreach ( var slot in DerivedPasswords )
                slot.Update( );
            _derivedPassword = DeriveDerivedPassword( );

            CopyText = DeriveCopyText( );
            _copyCommand.RaiseCanExecuteChanged( );

            _saveCommand.RaiseCanExecuteChanged( );
            _deleteCommand.RaiseCanExecuteChanged( );
        }

        private void Refresh( )
        {
            RaisePropertyChanged( ( ) => Key );
            RaisePropertyChanged( ( ) => Note );
            RaisePropertyChanged( ( ) => Iteration );
            RaisePropertyChanged( ( ) => IsKeyReadonly );

            foreach ( var derivedPassword in DerivedPasswords )
                derivedPassword.Refresh( );

            RequiredGuidColor = ConvertGuid( _model.ExpectedMasterPasswordId );

            Update( );
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
        private readonly IDialogService _dialogService;
        private readonly IGuidToColorConverter _guidToColor;

        private string _title = NewTitle;

        private Color _requiredGuidColor = Colors.Transparent;
        private Color _actualGuidColor = Colors.Transparent;
        private string _masterPasswordHint = string.Empty;

        private readonly ObservableCollection<DerivedPasswordViewModel> _derivedPasswords;

        private string _copyText;

        private string _derivedPassword;

        private readonly IUpdatableCommand _saveCommand;
        private readonly IUpdatableCommand _deleteCommand;
        private readonly IUpdatableCommand _copyCommand;

        private readonly ICommand _closeSelfCommand;
        private readonly ICommand _closeAllCommand;
        private readonly ICommand _closeAllButSelfCommand;
        private readonly ICommand _closeToTheRightCommand;
        private readonly ICommand _closeInsecureCommand;

        private readonly ExclusiveDelayedScheduler _scheduler;

        public const string NewTitle = "(new)";
    }
}