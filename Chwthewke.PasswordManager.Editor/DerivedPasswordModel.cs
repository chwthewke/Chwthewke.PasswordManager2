using System;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    internal class DerivedPasswordModel : IDerivedPasswordModel
    {
        public DerivedPasswordModel( IPasswordDerivationEngine derivationEngine, Guid generator )
        {
            if ( derivationEngine == null )
                throw new ArgumentNullException( "derivationEngine" );

            _derivationEngine = derivationEngine;
            Generator = generator;
            IsLegacy = _derivationEngine.LegacyPasswordGeneratorIds.Contains( generator );
        }


        public void UpdateDerivedPassword( string key, SecureString masterPassword, int iteration )
        {
            if ( string.IsNullOrEmpty( key ) || masterPassword.Length == 0 )
                _derivedPassword = NullDerivedPassword.Instance;
            else
                _derivedPassword = _derivationEngine.Derive( new PasswordRequest( key, masterPassword, iteration, Generator ) );
        }

        public Guid Generator { get; private set; }

        public bool IsLegacy { get; private set; }

        public IDerivedPassword DerivedPassword
        {
            get { return _derivedPassword; }
        }

        private IDerivedPassword _derivedPassword = NullDerivedPassword.Instance;
        private readonly IPasswordDerivationEngine _derivationEngine;
    }

    internal class NullDerivedPassword : IDerivedPassword
    {
        public static readonly IDerivedPassword Instance = new NullDerivedPassword( );

        public string Password
        {
            get { return string.Empty; }
        }

        public PasswordDigest Digest
        {
            get { return null; }
        }
    }
}