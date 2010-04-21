using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class CommandsTest
    {
        private PasswordEditorViewModel _viewModel;
        private IPasswordEditor _editor;
        private Mock<IClipboardService> _clipboardServiceMock;
        private Mock<IPasswordStore> _storeMock;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            IContainer container = builder.Build( );

            _storeMock = new Mock<IPasswordStore>( );
            _editor = new PasswordManager.Editor.PasswordEditor( PasswordGenerators.All,
                                                                 container.Resolve<IPasswordDigester>( ),
                                                                 _storeMock.Object );

            _clipboardServiceMock = new Mock<IClipboardService>( );

            _viewModel = new PasswordEditorViewModel( _editor, _storeMock.Object, _clipboardServiceMock.Object );
        }

        [ Test ]
        public void CopyPasswordWhenCommandAvailable( )
        {
            // Setup
            _viewModel.Key = "abc";
            _viewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            PasswordSlotViewModel slot = _viewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( _viewModel.CopyCommand.CanExecute( null ), Is.True );
            // Exercise
            _viewModel.CopyCommand.Execute( null );
            // Verify
            _clipboardServiceMock.Verify( cs => cs.CopyToClipboard( It.Is<string>( s => s == slot.Content ) ) );
        }

        [ Test ]
        public void SavePasswordWhenAvailable( )
        {
            // Setup
            _viewModel.Key = "abc";
            _viewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            PasswordSlotViewModel slot = _viewModel.Slots[ 0 ];
            slot.IsSelected = true;
            Assert.That( _viewModel.SaveCommand.CanExecute( null ), Is.True );
            // Exercise
            _viewModel.SaveCommand.Execute( null );
            // Verify
            _storeMock.Verify( store => store.AddOrUpdate( 
                It.Is<PasswordDigest>( d => d == _editor.GeneratedPassword( slot.Generator ).SavablePasswordDigest ) ) );
        }
    }
}