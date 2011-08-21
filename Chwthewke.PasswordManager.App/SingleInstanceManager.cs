using Microsoft.VisualBasic.ApplicationServices;

namespace Chwthewke.PasswordManager.App
{
    public class SingleInstanceManager : WindowsFormsApplicationBase
    {
        public SingleInstanceManager( PasswordManagerApp application )
        {
            _application = application;
            IsSingleInstance = true;
        }

        protected override bool OnStartup( StartupEventArgs eventArgs )
        {
            // First time _application is launched
            _application.Start( );
            return false;
        }

        protected override void OnStartupNextInstance( StartupNextInstanceEventArgs eventArgs )
        {
            // Subsequent launches
            base.OnStartupNextInstance( eventArgs );
            _application.Activate( );
        }

        private readonly PasswordManagerApp _application;
    }
}