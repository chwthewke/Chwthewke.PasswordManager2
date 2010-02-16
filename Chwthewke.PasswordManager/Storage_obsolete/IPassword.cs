using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal interface IPassword
    {
        bool HasPassword { get; }
        PasswordType PasswordType { get; }
        bool ValidatePassword( string generatedPassword );
        void SetPassword( string generatedPassword, PasswordType passwordType );
        void Clear( );
    }
}