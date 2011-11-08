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
        public PasswordEditorModel( IPasswordCollection passwordCollection,
                                    IPasswordDerivationEngine derivationEngine,
                                    IMasterPasswordMatcher masterPasswordMatcher )
            : this( new NewPasswordDocument( ), passwordCollection, derivationEngine, masterPasswordMatcher )
        {
        }

        public PasswordEditorModel( IPasswordCollection passwordCollection,
                                    IPasswordDerivationEngine derivationEngine,
                                    IMasterPasswordMatcher masterPasswordMatcher,
                                    PasswordDigestDocument original )
            : this( new BaselinePasswordDocument( original ), passwordCollection, derivationEngine, masterPasswordMatcher )
        {
        }

        private PasswordEditorModel( IBaselinePasswordDocument original,
                                     IPasswordCollection passwordCollection,
                                     IPasswordDerivationEngine derivationEngine,
                                     IMasterPasswordMatcher masterPasswordMatcher )
        {
            _passwordCollection = passwordCollection;
            _derivationEngine = derivationEngine;
            _masterPasswordMatcher = masterPasswordMatcher;

            _derivedPasswords = _derivationEngine.PasswordGenerators
                .Select<Guid, IDerivedPasswordModel>( g =>
                                                      new DerivedPasswordModel( _derivationEngine, this, g ) )
                .ToList( );

            _original = original;

            _key = original.Key;
            Note = _original.Note;
            Iteration = original.Iteration;

            MasterPassword = new SecureString( );
            SelectedPassword = DerivedPasswords.SingleOrDefault( p => p.Generator == _original.PasswordGenerator );
        }

        private string _key;

        public string Key
        {
            get { return _key; }
            set
            {
                if ( !IsKeyReadonly )
                    _key = value;
            }
        }

        public SecureString MasterPassword { get; set; }

        public int Iteration { get; set; }

        public string Note { get; set; }

        public IList<IDerivedPasswordModel> DerivedPasswords
        {
            get { return _derivedPasswords; }
        }

        public IDerivedPasswordModel SelectedPassword { get; set; }

        public Guid? MasterPasswordId
        {
            get { return _masterPasswordMatcher.IdentifyMasterPassword( MasterPassword ); }
        }

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
                       SelectedPassword.Generator != _original.PasswordGenerator ||
                       Iteration != _original.Iteration;
            }
        }

        public bool CanSave
        {
            get { return CanSaveWithMasterPassword( ) || CanSaveWithoutMasterPassword( ); }
        }

        public bool CanDelete
        {
            get { return IsKeyReadonly; }
        }

        public bool Save( )
        {
            throw new NotImplementedException( );
        }

        public bool Delete( )
        {
            throw new NotImplementedException( );
        }

        private bool CanSaveWithMasterPassword( )
        {
            return ( IsDirty || MasterPasswordId != _original.MasterPasswordId ) &&
                   MasterPassword.Length > 0 &&
                   !string.IsNullOrEmpty( Key ) &&
                   SelectedPassword != null;
        }

        private bool CanSaveWithoutMasterPassword( )
        {
            return IsDirty &&
                   MasterPassword.Length == 0 &&
                   IsKeyReadonly &&
                   SelectedPassword.Generator == _original.PasswordGenerator &&
                   Iteration == _original.Iteration;
        }

        private PasswordDigestDocument Current
        {
            get
            {
                if ( SelectedPassword == null )
                    return null;
                if ( SelectedPassword.DerivedPassword == null )
                    return null;
                DateTime createdOn = _original.CreatedOn ?? Now;
                Guid masterPasswordId = MasterPasswordId ?? Guid.NewGuid( );
                return new PasswordDigestDocument( SelectedPassword.DerivedPassword.Digest, masterPasswordId, createdOn, Now, Note );
            }
        }


        private DateTime Now
        {
            get { return DateTime.Now; }
        }

        private readonly IBaselinePasswordDocument _original;

        private readonly IPasswordCollection _passwordCollection;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IList<IDerivedPasswordModel> _derivedPasswords;
    }
}