using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal interface IPasswordEditorModelFactory
    {
        IPasswordEditorModel CreateModel( PasswordDigestDocument password );
    }
}