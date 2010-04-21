using Autofac;
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
    [ TestFixture ]
    public class LoadingKeyTest
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
        public void LoadEnablesWhenTypingStoredKey( )
        {
            // Setup
            _storeMock.Setup( s => s.Passwords ).Returns( new PasswordDigest[ ]
                                                              { new PasswordDigestBuilder( ).WithKey( "abcd" ) } );
            // Exercise
            _viewModel.Key = "abcd";
            // Verify
            Assert.That( _viewModel.LoadEnabled, Is.True );
        }

        [ Test ]
        public void LoadDoesNotEnableUntilStoredKeyFullyTyped( )
        {
            // Setup
            _storeMock.Setup( s => s.Passwords ).Returns( new PasswordDigest[ ]
                                                              { new PasswordDigestBuilder( ).WithKey( "abcd" ) } );
            // Exercise
            _viewModel.Key = "abc";
            // Verify
            Assert.That( _viewModel.LoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadDoesNotEnableAfterStoredKeyTyped( )
        {
            // Setup
            _storeMock.Setup( s => s.Passwords ).Returns( new PasswordDigest[ ]
                                                              { new PasswordDigestBuilder( ).WithKey( "abcd" ) } );
            // Exercise
            _viewModel.Key = "abcd";
            _viewModel.Key = "abcde";
            // Verify
            Assert.That( _viewModel.LoadEnabled, Is.False );
        }

        [ Test ]
        public void ExecuteLoadRaisesLoadRequestedEvent( )
        {
            // Setup
            _storeMock.Setup( s => s.Passwords ).Returns( new PasswordDigest[ ] { new PasswordDigestBuilder( ).WithKey( "abcd" ) } );
            _viewModel.Key = "abcd";
            bool loadWasRequested = false;
            _viewModel.LoadRequested += ( s, e ) => { if ( s == _viewModel ) loadWasRequested = true; };
            // Exercise
            _viewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( loadWasRequested, Is.True );
        }
    }
}