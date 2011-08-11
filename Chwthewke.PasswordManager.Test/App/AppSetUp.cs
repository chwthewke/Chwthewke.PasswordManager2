using Autofac;
using Chwthewke.PasswordManager.App.Modules;

namespace Chwthewke.PasswordManager.Test.App
{
    class AppSetUp
    {
        public static IContainer Container
        {
            get { return AppConfiguration.ConfigureContainer( ); }
        }
    }
}
