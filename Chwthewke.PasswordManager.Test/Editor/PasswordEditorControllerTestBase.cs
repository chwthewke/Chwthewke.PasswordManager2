using System;
using System.Security;
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

        protected IPasswordEditorController Controller;

        protected Mock<ITimeProvider> TimeProviderMock;

        protected readonly DateTime Now = new DateTime( 2011, 8, 15 );


        [ SetUp ]
        public void SetUpDependencies( )
        {
            TimeProviderMock = new Mock<ITimeProvider>( );
            TimeProviderMock.Setup( p => p.Now ).Returns( Now );

            AppSetUp.TestContainer(
                b =>
                    {
                        b.RegisterInstance( TimeProviderMock.Object ).As<ITimeProvider>( );
                        b.RegisterType<InMemoryTextResource>( ).As<ITextResource>( ).SingleInstance( );
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