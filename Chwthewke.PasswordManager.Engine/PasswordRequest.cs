using System;
using System.Security;

namespace Chwthewke.PasswordManager.Engine
{
    public class PasswordRequest
    {
        public PasswordRequest( string key, SecureString masterPassword, int iterations, 
            Guid passwordGenerator, int digestIterations, int digestLength )
        {
            Key = key;
            MasterPassword = masterPassword;
            Iterations = iterations;
            PasswordGenerator = passwordGenerator;
            DigestIterations = digestIterations;
            DigestLength = digestLength;
        }

        public string Key { get; private set; }
        public SecureString MasterPassword { get; set; }
        public int Iterations { get; private set; }
        public Guid PasswordGenerator { get; private set; }
        public int DigestIterations { get; private set; }
        public int DigestLength { get; private set; }
    }
}