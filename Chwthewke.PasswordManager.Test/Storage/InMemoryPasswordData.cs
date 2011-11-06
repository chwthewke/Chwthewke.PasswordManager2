using System.Collections.Generic;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class InMemoryPasswordData : IPasswordData
    {
        public IEnumerable<PasswordDigestDocument> LoadPasswords( )
        {
            return Passwords;
        }

        public void SavePasswords( IEnumerable<PasswordDigestDocument> passwords )
        {
            Passwords = passwords;
        }

        private IEnumerable<PasswordDigestDocument> Passwords { get; set; }
    }
}