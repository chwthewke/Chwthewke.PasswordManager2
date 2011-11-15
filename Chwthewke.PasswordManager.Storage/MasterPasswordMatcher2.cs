using System;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class MasterPasswordMatcher2 : IMasterPasswordMatcher
    {
        public MasterPasswordMatcher2( IPasswordDerivationEngine passwordDerivationEngine, IPasswordRepository passwordRepository )
        {
            _passwordDerivationEngine = passwordDerivationEngine;
            _passwordRepository = passwordRepository;
        }

        private readonly IPasswordDerivationEngine _passwordDerivationEngine;
        private readonly IPasswordRepository _passwordRepository;

        public Guid? IdentifyMasterPassword( SecureString masterPassword )
        {
            if ( masterPassword.Length == 0 )
                return null;
            return _passwordRepository.LoadPasswords( )
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