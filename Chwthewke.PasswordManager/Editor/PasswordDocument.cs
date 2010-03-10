using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public class PasswordDocument : IPasswordDocument
    {
        public PasswordDocument( string generatedPassword )
        {
            _generatedPassword = generatedPassword;
        }

        public string GeneratedPassword
        {
            get { return _generatedPassword; }
        }

        public PasswordDigest SavablePasswordDigest
        {
            get { throw new NotImplementedException( ); }
        }

        private readonly string _generatedPassword;
    }
}