using System;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class MasterPasswordMatcher2 : IMasterPasswordMatcher
    {
        public MasterPasswordMatcher2( IPasswordDerivationEngine passwordDerivationEngine, IPasswordCollection passwordCollection )
        {
            _passwordDerivationEngine = passwordDerivationEngine;
            _passwordCollection = passwordCollection;
        }

        private readonly IPasswordDerivationEngine _passwordDerivationEngine;
        private readonly IPasswordCollection _passwordCollection;

        public Guid? IdentifyMasterPassword( SecureString masterPassword )
        {
            return _passwordCollection.LoadPasswords( )
                .GroupBy( p => p.MasterPasswordId )
                .Select( gr => gr.First( ) )
                .Where( p => MasterPasswordMatches( masterPassword, p ) )
                .Select( p => (Guid?) p.MasterPasswordId )
                .FirstOrDefault( );
        }

        private bool MasterPasswordMatches( SecureString masterPassword, PasswordDigestDocument password )
        {
            PasswordRequest testRequest =
                new PasswordRequest( password.Key, masterPassword, password.Iteration, password.PasswordGenerator );
            var testDigest = _passwordDerivationEngine.Derive( testRequest ).Digest;
            
            return testDigest.Equals( password.Digest );
        }
    }
}