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
        protected IPasswordEditor Editor;
        protected Mock<IClipboardService> ClipboardServiceMock;
        protected Mock<IPasswordStore> StoreMock;

        [SetUp]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            IContainer container = builder.Build( );

            StoreMock = new Mock<IPasswordStore>( );
            Editor = new PasswordManager.Editor.PasswordEditor( PasswordGenerators.All,
                                                                 container.Resolve<IPasswordDigester>( ),
                                                                 StoreMock.Object );

            ClipboardServiceMock = new Mock<IClipboardService>( );

            ViewModel = new PasswordEditorViewModel( Editor, StoreMock.Object, ClipboardServiceMock.Object );
        }

    }
}