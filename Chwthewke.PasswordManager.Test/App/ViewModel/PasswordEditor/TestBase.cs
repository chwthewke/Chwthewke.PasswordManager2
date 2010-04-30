using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Moq;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    public class TestBase
    {
        protected PasswordEditorViewModel ViewModel;
        protected IPasswordEditorController Controller;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            IContainer container = builder.Build( );

            Controller = container.Resolve<IPasswordEditorController>( );

            var clipboardServiceMock = new Mock<IClipboardService>( );

            ViewModel = new PasswordEditorViewModel( Controller, clipboardServiceMock.Object, 
                PasswordGenerators.All.Select( g => new PasswordSlotViewModel( g ) ) );
        }
    }
}