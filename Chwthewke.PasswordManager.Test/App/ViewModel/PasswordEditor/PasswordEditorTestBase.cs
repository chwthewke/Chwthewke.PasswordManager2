using System;
using System.Linq;
using System.Security;
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
        protected Mock<IClipboardService> ClipboardServiceMock;

// ReSharper disable UnusedAutoPropertyAccessor.Global
        public PasswordEditorViewModel ViewModel { get; set; }
        public IPasswordEditorController Controller { get; set; }
        public IGuidToColorConverter GuidToColorConverter { get; set; }
        public IPasswordDatabase PasswordDatabase { get; set; }
        public Func<IPasswordEditorController> ControllerFactory { get; set; }
// ReSharper restore UnusedAutoPropertyAccessor.Global


        [SetUp]
        public void SetUpPasswordEditorViewModel( )
        {
            ClipboardServiceMock = new Mock<IClipboardService>( );

            AppSetUp.TestContainer( b => b.RegisterInstance( ClipboardServiceMock.Object ).As<IClipboardService>( ) )
                .InjectProperties( this );
        }

        protected void AddPassword( string key, string note, IPasswordGenerator generator, SecureString masterPassword )
        {
            IPasswordEditorController controller = ControllerFactory.Invoke( );
            controller.Key = key;
            controller.Note = note;
            controller.SelectedGenerator = generator;
            controller.MasterPassword = masterPassword;

            controller.SavePassword( );
        }
    }
}