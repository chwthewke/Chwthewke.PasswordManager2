using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Autofac;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.App;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    public class PasswordEditorControllerTestBase
    {
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public IPasswordDatabase PasswordDatabase { get; set; }

        public IPasswordDigester Digester { get; set; }

        public PasswordEditorControllerFactory ControllerFactory { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global

        protected IPasswordEditorController _controller;

        protected Mock<ITimeProvider> _timeProviderMock;

        protected readonly DateTime _now = new DateTime( 2011, 8, 15 );


        [SetUp]
        public void SetUpDependencies( )
        {
            _timeProviderMock = new Mock<ITimeProvider>( );
            _timeProviderMock.Setup( p => p.Now ).Returns( _now );

            AppSetUp.TestContainer(
                b =>
                    {
                        b.RegisterInstance( _timeProviderMock.Object ).As<ITimeProvider>( );
                        b.RegisterType<InMemoryPasswordStore>( ).As<IPasswordStore>( ).SingleInstance( );
                    } )
                .InjectProperties( this );
        }

        protected Guid StorePasswordAndGetMasterPasswordId( string key, IPasswordGenerator generator, SecureString masterPassword )
        {
            IPasswordEditorController controller = ControllerFactory.PasswordEditorControllerFor( string.Empty );
            controller.Key = key;
            controller.SelectedGenerator = generator;
            controller.MasterPassword = masterPassword;
            controller.SavePassword( );
            return PasswordDatabase.FindByKey( key ).MasterPasswordId;
        }

    }
}
