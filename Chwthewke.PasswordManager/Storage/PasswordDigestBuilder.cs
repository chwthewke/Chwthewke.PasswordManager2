using System;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordDigestBuilder
    {
        public PasswordDigestBuilder( string key, string generatedPassword, Guid masterPasswordId, Guid passwordGeneratorId )
        {
            _key = key;
            _generatedPassword = generatedPassword;
            _masterPasswordId = masterPasswordId;
            _passwordGeneratorId = passwordGeneratorId;
        }

        public string Note { get; set; }

        public PasswordDigest Build( )
        {
            throw new NotImplementedException( );
        }

        private readonly string _key;
        private readonly string _generatedPassword;
        private readonly Guid _masterPasswordId;
        private readonly Guid _passwordGeneratorId;
    }
}