using System;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public class DerivedPasswordModel : IDerivedPasswordModel
    {
        public DerivedPasswordModel( IPasswordDerivationEngine derivationEngine, IPasswordEditorModel editorModel, Guid generator )
        {
            if ( derivationEngine == null ) 
                throw new ArgumentNullException( "derivationEngine" );
            if ( editorModel == null )
                throw new ArgumentNullException( "editorModel" );

            _derivationEngine = derivationEngine;
            _editorModel = editorModel;
            Generator = generator;
            Iteration = 1;
        }

        public Guid Generator { get; private set; }

        public DerivedPassword DerivedPassword
        {
            get
            {
                string key = _editorModel.Key;
                SecureString masterPassword = _editorModel.MasterPassword;
                if ( string.IsNullOrEmpty( key ) || masterPassword.Length == 0 )
                    return null;
                return _derivationEngine.Derive( new PasswordRequest( key, masterPassword, Iteration, Generator ) );
            }
        }

        public int Iteration { get; set; }

        private IPasswordEditorModel _editorModel;
        private IPasswordDerivationEngine _derivationEngine;
    }
}