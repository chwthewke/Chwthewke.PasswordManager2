using System.Collections.Generic;
using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using ITimeProvider = Chwthewke.PasswordManager.Editor.ITimeProvider;

namespace Chwthewke.PasswordManager.App.Modules
{
    public class PasswordManagerModule : Module
    {
        protected override void Load( ContainerBuilder builder )
        {
            builder.Register( CreateEditor ).As<IPasswordManagerEditor>( ).SingleInstance( );

            builder.RegisterInstance( PasswordManagerEngine.DerivationEngine ).As<IPasswordDerivationEngine>( );

            builder.Register( CreateStorage ).As<IPasswordManagerStorage>( ).SingleInstance( );

        }

        private IPasswordManagerStorage CreateStorage( IComponentContext c )
        {
            return PasswordManagerStorage.CreateService( new NullPasswordData() );
        }

        private IPasswordManagerEditor CreateEditor( IComponentContext c )
        {
            return PasswordManagerEditor.CreateService( c.Resolve<IPasswordDerivationEngine>( ),
                                                        c.Resolve<IPasswordManagerStorage>( ),
                                                        c.Resolve<ITimeProvider>( ) );
        }
    }

    // null object for stubbing at init time
    internal class NullPasswordData : IPasswordData
    {
        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            return new List<PasswordDigestDocument>( );
        }

        public void SavePasswords( IList<PasswordDigestDocument> passwords )
        {
        }
    }
}