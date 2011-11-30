using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorModel : IPasswordEditorModel
    {
        public PasswordEditorModel( IPasswordRepository passwordRepository,
                                    IPasswordDerivationEngine derivationEngine,
                                    IMasterPasswordMatcher masterPasswordMatcher,
                                    ITimeProvider timeProvider )
            : this( passwordRepository, derivationEngine, masterPasswordMatcher, timeProvider, new NewPasswordDocument( ) )
        {
        }

        public PasswordEditorModel( IPasswordRepository passwordRepository,
                                    IPasswordDerivationEngine derivationEngine,
                                    IMasterPasswordMatcher masterPasswordMatcher,
                                    ITimeProvider timeProvider,
                                    PasswordDigestDocument original )
            : this( passwordRepository, derivationEngine, masterPasswordMatcher, timeProvider, new BaselinePasswordDocument( original ) )
        {
        }

        // TODO factory delegate for autofac

        private PasswordEditorModel( IPasswordRepository passwordRepository,
                                     IPasswordDerivationEngine derivationEngine,
                                     IMasterPasswordMatcher masterPasswordMatcher,
                                     ITimeProvider timeProvider,
                                     IBaselinePasswordDocument original )
        {
            _passwordRepository = passwordRepository;
            _derivationEngine = derivationEngine;
            _masterPasswordMatcher = masterPasswordMatcher;
            _timeProvider = timeProvider;

            _derivedPasswords = _derivationEngine.PasswordGenerators
                .Select( g => new DerivedPasswordModel( _derivationEngine, g ) )
                .ToList( );


            _original = original;
            UpdateFromOriginal( );

            MasterPassword = new SecureString( );
        }


        public string Key
        {
            get { return _key; }
            set
            {
                if ( IsKeyReadonly )
                    return;
                _key = value;
            }
        }

        public SecureString MasterPassword
        {
            get { return _masterPassword; }
            set
            {
                _masterPassword = value;
                UpdateMasterPasswordId( );
            }
        }

        private void UpdateMasterPasswordId( )
        {
            MasterPasswordId = _masterPasswordMatcher.IdentifyMasterPassword( _masterPassword );
        }

        public int Iteration
        {
            get { return _iteration; }
            set
            {
                _iteration = value;
            }
        }


        public string Note { get; set; }

        public IEnumerable<IDerivedPasswordModel> DerivedPasswords
        {
            get { return _derivedPasswords; }
        }

        public IDerivedPasswordModel SelectedPassword { get; set; }

        public Guid? MasterPasswordId { get; private set; }

        public Guid? ExpectedMasterPasswordId
        {
            get { return _original.MasterPasswordId; }
        }

        public bool IsKeyReadonly
        {
            get { return !string.IsNullOrEmpty( _original.Key ); }
        }

        public bool IsDirty
        {
            get
            {
                return Key != _original.Key ||
                       Note != _original.Note ||
                       SelectedGenerator != _original.PasswordGenerator ||
                       Iteration != _original.Iteration;
            }
        }

        private Guid? SelectedGenerator
        {
            get { return SelectedPassword == null ? (Guid?) null : SelectedPassword.Generator; }
        }

        public bool CanSave
        {
            get { return CanSaveWithMasterPassword || CanSaveWithoutMasterPassword; }
        }

        public bool CanDelete
        {
            get { return IsKeyReadonly; }
        }

        public bool Save( )
        {
            bool saveOrUpdate = SaveOrUpdate( );
            if ( saveOrUpdate )
            {
                _original = new BaselinePasswordDocument( _passwordRepository.LoadPassword( Key ) );
                UpdateMasterPasswordId( );
            }
            return saveOrUpdate;
        }

        public bool Delete( )
        {
            if ( !CanDelete )
                return false;
            bool deleted = DeletePassword( );
            if ( !deleted )
                return false;

            _original = new NewPasswordDocument( );
            UpdateMasterPasswordId( );

            return true;
        }

        public void UpdateDerivedPasswords( )
        {
            foreach ( var derivedPasswordModel in _derivedPasswords )
                derivedPasswordModel.UpdateDerivedPassword( Key, MasterPassword, Iteration );
        }

        private void UpdateFromOriginal( )
        {
            _key = _original.Key;
            Note = _original.Note;
            _iteration = _original.Iteration;
            SelectedPassword = _derivedPasswords.SingleOrDefault( p => p.Generator == _original.PasswordGenerator );
        }

        private bool CanSaveWithMasterPassword
        {
            get
            {
                return ( IsDirty || MasterPasswordId != _original.MasterPasswordId ) &&
                       MasterPassword.Length > 0 &&
                       !string.IsNullOrEmpty( Key ) &&
                       SelectedPassword != null;
            }
        }

        private bool CanSaveWithoutMasterPassword
        {
            get
            {
                return IsDirty &&
                       MasterPassword.Length == 0 &&
                       IsKeyReadonly &&
                       SelectedPassword.Generator == _original.PasswordGenerator &&
                       Iteration == _original.Iteration;
            }
        }

        private bool SaveOrUpdate( )
        {
            if ( CanSaveWithoutMasterPassword )
                return SaveUpdatedNote( );
            if ( CanSaveWithMasterPassword )
                return SaveFullUpdate( );
            return false;
        }

        private bool SaveFullUpdate( )
        {
            if ( SelectedPassword == null )
                throw new InvalidOperationException( );
            DateTime createdOn = _original.CreatedOn ?? Now;
            Guid masterPasswordId = MasterPasswordId ?? Guid.NewGuid( );
            PasswordDigestDocument current =
                new PasswordDigestDocument( SelectedPassword.DerivedPassword.Digest, masterPasswordId, createdOn, Now, Note );
            return Save( current );
        }


        private bool SaveUpdatedNote( )
        {
            Guid? masterPasswordId = _original.MasterPasswordId;
            DateTime? createdOn = _original.CreatedOn;
            PasswordDigestDocument originalDocument = _original.Document;

            if ( !masterPasswordId.HasValue )
                throw new InvalidOperationException( );
            if ( !createdOn.HasValue )
                throw new InvalidOperationException( );
            if ( originalDocument == null )
                throw new InvalidOperationException( );

            PasswordDigestDocument current =
                new PasswordDigestDocument( originalDocument.Digest, masterPasswordId.Value, createdOn.Value, Now, Note );
            return Save( current );
        }

        private bool Save( PasswordDigestDocument passwordDigestDocument )
        {
            if ( _original.Document != null )
                return _passwordRepository.UpdatePassword( _original.Document, passwordDigestDocument );
            return _passwordRepository.SavePassword( passwordDigestDocument );
        }

        private bool DeletePassword( )
        {
            return _passwordRepository.DeletePassword( _original.Document, Now );
        }

        public void Reload( )
        {
            PasswordDigestDocument passwordDigestDocument = _passwordRepository.LoadPassword( Key );
            if ( passwordDigestDocument == null )
                _original = new NewPasswordDocument( );
            else
            {
                var update = !IsDirty;
                _original = new BaselinePasswordDocument( passwordDigestDocument );
                if ( update )
                    UpdateFromOriginal( );
            }
            UpdateMasterPasswordId( );
        }

        private DateTime Now
        {
            get { return _timeProvider.Now; }
        }

        private string _key;
        private int _iteration;
        private SecureString _masterPassword = new SecureString();

        private IBaselinePasswordDocument _original;

        private readonly ITimeProvider _timeProvider;

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IList<DerivedPasswordModel> _derivedPasswords;
    }
}