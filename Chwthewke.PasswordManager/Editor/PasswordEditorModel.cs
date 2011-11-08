using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorModel : IPasswordEditorModel
    {
        public PasswordEditorModel( IPasswordCollection passwordCollection, IPasswordDerivationEngine derivationEngine,
                                    IMasterPasswordMatcher masterPasswordMatcher )
        {
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
            MasterPassword = new SecureString( );
            SelectedPassword = DerivedPasswords.Single( p => p.Generator == _original.PasswordGenerator );

            foreach ( IDerivedPasswordModel derivedPassword in DerivedPasswords )
                derivedPassword.Iteration = _original.Iteration;
        }

        public string Key { get; set; }

        public SecureString MasterPassword { get; set; }

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
            get { return false; }
        }

        public bool Save( )
        {
            throw new NotImplementedException( );
        }

        public bool Delete( )
        {
            throw new NotImplementedException( );
        }

        private PasswordDigestDocument _original;

        private readonly IPasswordCollection _passwordCollection;
        private readonly IPasswordDerivationEngine _derivationEngine;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IList<IDerivedPasswordModel> _derivedPasswords;
    }
}