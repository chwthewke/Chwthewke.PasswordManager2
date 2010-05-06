using System.Linq;
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
    public abstract class PasswordEditorTestBase
    {
        protected PasswordEditorViewModel ViewModel;
        protected IPasswordEditorController Controller;
        protected Mock<IClipboardService> ClipboardServiceMock;
        protected IContainer Container;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            builder.Register( c => c.Resolve<IPasswordEditorControllerFactory>( ).CreatePasswordEditorController( ) )
                .As<IPasswordEditorController>( );

            Container = builder.Build( );

            Controller = Container.Resolve<IPasswordEditorController>( );

            ClipboardServiceMock = new Mock<IClipboardService>( );

            ViewModel = new PasswordEditorViewModel( Controller, ClipboardServiceMock.Object,
                                                     PasswordGenerators.All.Select( g => new PasswordSlotViewModel( g ) ) );
        }

        protected IPasswordStore PasswordStore
        {
            get { return Container.Resolve<IPasswordStore>( ); }
        }
    }
}