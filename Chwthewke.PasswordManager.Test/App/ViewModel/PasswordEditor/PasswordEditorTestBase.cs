using System.Linq;
using Autofac;
using Chwthewke.PasswordManager.App.Modules;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Storage;
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
        protected IGuidToColorConverter GuidToColorConverter;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            builder.RegisterModule( new PasswordStorageModule( ) );
            builder.RegisterModule( new ApplicationServices( ) );

            Container = builder.Build( );

            Controller = Container.Resolve<IPasswordEditorController>( );

            ClipboardServiceMock = new Mock<IClipboardService>( );

            GuidToColorConverter = Container.Resolve<IGuidToColorConverter>( );
            ViewModel = new PasswordEditorViewModel( Controller, ClipboardServiceMock.Object,
                                                     PasswordGenerators.All.Select( g => new PasswordSlotViewModel( g ) ),
                                                     GuidToColorConverter );
        }

        protected IPasswordRepository PasswordRepository
        {
            get { return Container.Resolve<IPasswordRepository>( ); }
        }
    }
}