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
        public PasswordEditorModel( IPasswordCollection passwordCollection, IPasswordDerivationEngine derivationEngine,
                                    IMasterPasswordMatcher masterPasswordMatcher )
        {
            Key = string.Empty;
            MasterPassword = new SecureString( );
            Note = string.Empty;

            _passwordCollection = passwordCollection;
            _derivationEngine = derivationEngine;
            _masterPasswordMatcher = masterPasswordMatcher;
            _derivedPasswords = _derivationEngine.PasswordGenerators
                .Select<Guid, IDerivedPasswordModel>( g => new DerivedPasswordModel( _derivationEngine, this, g ) )
                .ToList( );
        }

        public PasswordEditorModel( PasswordDigestDocument original, IPasswordCollection passwordCollection,
                                    IPasswordDerivationEngine derivationEngine, IMasterPasswordMatcher masterPasswordMatcher )
            : this( passwordCollection, derivationEngine, masterPasswordMatcher )
        {
            _original = original;
            Key = _original.Key;
            SelectedPassword = DerivedPasswords.SingleOrDefault( p => p.Generator == _original.PasswordGenerator );

            foreach ( IDerivedPasswordModel derivedPassword in DerivedPasswords )
                derivedPassword.Iteration = _original.Iteration;
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
            get { return _original == null ? null : (Guid?) _original.MasterPasswordId; }
        }

        public bool IsKeyReadonly
        {
            get { return _original != null; }
        }

        public bool IsDirty
        {
            get { return false; }
        }

        public bool CanSave
        {
            get { return false; }
        }

        public bool CanDelete
        {
            get { return true; }
        }

        public bool Save( )
        {
            throw new NotImplementedException( );
        }

        public bool Delete( )
        {
            throw new NotImplementedException( );
        }

        private PasswordDigestDocument Current
        {
            get
            {
                if ( SelectedPassword == null )
                    return null;
                if ( SelectedPassword.DerivedPassword == null )
                    return null;
                var createdOn = _original == null ? Now : _original.CreatedOn;
                Guid masterPasswordId = MasterPasswordId ?? Guid.NewGuid( );
                return new PasswordDigestDocument( SelectedPassword.DerivedPassword.Digest, masterPasswordId, createdOn, Now, Note );
            }
        }


        private DateTime Now
        {
            get { return DateTime.Now; }
        }

        private readonly PasswordDigestDocument _original;

        private readonly IPasswordCollection _passwordCollection;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IList<IDerivedPasswordModel> _derivedPasswords;
    }
}