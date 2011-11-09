using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    internal class DerivedPasswordModel : IDerivedPasswordModel
    {
        public DerivedPasswordModel( IPasswordDerivationEngine derivationEngine, IPasswordEditorModel editorModel, 
            Guid generator )
        {
            if ( derivationEngine == null ) 
                throw new ArgumentNullException( "derivationEngine" );
            if ( editorModel == null )
                throw new ArgumentNullException( "editorModel" );

            _derivationEngine = derivationEngine;
            _editorModel = editorModel;
            Generator = generator;
        }

        public Guid Generator { get; private set; }

        public IDerivedPassword DerivedPassword
        {
            get
            {
                string key = _editorModel.Key;
                SecureString masterPassword = _editorModel.MasterPassword;
                if ( string.IsNullOrEmpty( key ) || masterPassword.Length == 0 )
                    return NullDerivedPassword.Instance;

                int iteration = _editorModel.Iteration;
                return _derivationEngine.Derive( new PasswordRequest( key, masterPassword, iteration, Generator ) );
            }
        }

        private readonly IPasswordEditorModel _editorModel;
        private readonly IPasswordDerivationEngine _derivationEngine;
    }

    internal class NullDerivedPassword : IDerivedPassword
    {
        public static readonly IDerivedPassword Instance = new NullDerivedPassword( );

        public string Password
        {
            get { return string.Empty; }
        }

        public PasswordDigest2 Digest
        {
            get { return null; }
        }
    }
}