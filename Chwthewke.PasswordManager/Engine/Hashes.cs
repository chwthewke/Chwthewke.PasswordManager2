using System;
using System.Security.Cryptography;

namespace Chwthewke.PasswordManager.Engine
{
    public static class Hashes
    {
        public static IHashFactory Sha512Factory
        {
            get { return _sha512Factory; }
        }

        private static readonly IHashFactory _sha512Factory = new Sha512Factory( );
    }
}