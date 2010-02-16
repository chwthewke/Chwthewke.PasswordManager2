using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordData
    {
        public PasswordType Type { get; set; }

        public byte[ ] Hash { get; set; }
    }
}