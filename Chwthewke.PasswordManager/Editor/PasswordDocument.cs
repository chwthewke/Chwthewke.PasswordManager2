using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public class PasswordDocument
    {
        public PasswordDocument( string generatedPassword, PasswordDigest digest )
        {
            if ( generatedPassword == null )
                throw new ArgumentNullException( "generatedPassword" );
            if ( digest == null )
                throw new ArgumentNullException( "digest" );

            _generatedPassword = generatedPassword;
            _digest = digest;
        }

        public string GeneratedPassword
        {
            get { return _generatedPassword; }
        }

        public PasswordDigest SavablePasswordDigest
        {
            get { return _digest; }
        }

        private readonly string _generatedPassword;
        private readonly PasswordDigest _digest;
    }
}