using System.Collections.Generic;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class InMemoryPasswordData : IPasswordData
    {
        public InMemoryPasswordData( )
        {
            Passwords = new List<PasswordDigestDocument>( );
        }

        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            return Passwords;
        }

        public void SavePasswords( IList<PasswordDigestDocument> passwords )
        {
            Passwords = passwords;
        }

        private IList<PasswordDigestDocument> Passwords { get; set; }
    }
}