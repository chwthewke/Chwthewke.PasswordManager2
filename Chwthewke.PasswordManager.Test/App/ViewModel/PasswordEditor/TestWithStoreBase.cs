using System.Security;
using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    public class TestWithStoreBase
    {
        protected PasswordEditorViewModel ViewModel;
        protected IPasswordEditorController Editor;
        protected Mock<IClipboardService> ClipboardServiceMock;
        private IContainer _container;
        //protected Mock<IPasswordStore> StoreMock;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            _container = builder.Build( );

            Editor = _container.Resolve<IPasswordEditorController>( );

            ClipboardServiceMock = new Mock<IClipboardService>( );

            ViewModel = new PasswordEditorViewModel( Editor, ClipboardServiceMock.Object );
        }

        protected IPasswordStore PasswordStore
        {
            get { return _container.Resolve<IPasswordStore>( ); }
        }

        protected void AddPassword( string key, string note, IPasswordGenerator generator, SecureString masterPassword )
        {
            IPasswordEditorController controller = _container.Resolve<IPasswordEditorController>( );
            controller.Key = key;
            controller.Note = note;
            controller.SelectedGenerator = generator;
            controller.MasterPassword = masterPassword;

            controller.SavePassword( );
        }
    }
}