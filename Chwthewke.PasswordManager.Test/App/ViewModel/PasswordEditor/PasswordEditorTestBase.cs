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
        private IContainer _container;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            builder.Register( c => c.Resolve<IPasswordEditorControllerFactory>( ).CreatePasswordEditorController( ) )
                .As<IPasswordEditorController>( );

            _container = builder.Build( );

            Controller = _container.Resolve<IPasswordEditorController>( );

            ClipboardServiceMock = new Mock<IClipboardService>( );

            ViewModel = new PasswordEditorViewModel( Controller, ClipboardServiceMock.Object,
                                                     PasswordGenerators.All.Select( g => new PasswordSlotViewModel( g ) ) );
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