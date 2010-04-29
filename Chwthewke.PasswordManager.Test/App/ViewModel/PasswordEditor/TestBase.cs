using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Modules;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    public class TestBase
    {
        protected PasswordEditorViewModel ViewModel;
        protected IPasswordEditorController Editor;
        protected Mock<IClipboardService> ClipboardServiceMock;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            IContainer container = builder.Build( );

            Editor = container.Resolve<IPasswordEditorController>( );

            ClipboardServiceMock = new Mock<IClipboardService>( );

            ViewModel = new PasswordEditorViewModel( Editor, ClipboardServiceMock.Object );
        }
    }
}