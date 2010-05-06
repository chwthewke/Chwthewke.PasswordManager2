using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordListTest
    {
        [ SetUp ]
        public void SetUpPasswordList( )
        {
            ContainerBuilder containerBuilder = new ContainerBuilder( );
            containerBuilder.RegisterModule( new PasswordManagerModule( ) );
            containerBuilder.RegisterType<ClipboardService>( ).As<IClipboardService>( );
            containerBuilder.RegisterType<PasswordEditorFactory>( ).As<IPasswordEditorFactory>( );
            _container = containerBuilder.Build( );
        }

        [ Test ]
        public void ListHasPasswords( )
        {
            // Setup
            _container.AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            _container.AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            _container.AddPassword( "abcd", string.Empty, PasswordGenerators.Full, "1234".ToSecureString( ) );
            // Exercise
            _passwordList = new PasswordListViewModel( _container.Resolve<IPasswordStore>( ),
                                                       _container.Resolve<IPasswordEditorFactory>( ) );
            // Verify
            Assert.That( _passwordList.Items.Select( x => x.Name ).ToArray( ),
                Is.EqualTo( new[ ] { "abc", "abcd", "abde" } ) );
        }

        private IContainer _container;
        private PasswordListViewModel _passwordList;
    }
}