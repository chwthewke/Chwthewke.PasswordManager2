using System.Security;
using Autofac;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    public static class TestSetupExtensions
    {
        public static void AddPassword( this IContainer container,
                                        string key,
                                        string note,
                                        IPasswordGenerator generator,
                                        SecureString masterPassword )
        {
            IPasswordEditorController controller = 
                container.Resolve<IPasswordEditorControllerFactory>( ).CreatePasswordEditorController( );
            controller.Key = key;
            controller.Note = note;
            controller.SelectedGenerator = generator;
            controller.MasterPassword = masterPassword;

            controller.SavePassword( );
        }
    }
}