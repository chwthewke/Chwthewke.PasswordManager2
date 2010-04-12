using System.Security;
using Autofac;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App.ViewModel
{
    [ TestFixture ]
    public class PasswordEditorViewModelTest
    {
        private PasswordEditorViewModel _viewModel;
        private IPasswordEditor _editor;

        [ SetUp ]
        public void SetUpPasswordEditorViewModel( )
        {
            ContainerBuilder builder = new ContainerBuilder( );
            builder.RegisterModule( new PasswordManagerModule( ) );
            IContainer container = builder.Build( );

            _editor = container.Resolve<IPasswordEditor>( );

            _viewModel = new PasswordEditorViewModel( _editor );
        }

        [ Test ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( _viewModel.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _viewModel.Title, Is.EqualTo( "(new)" ) );
            Assert.That( _viewModel.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _viewModel.SaveCommand.CanExecute( new SecureString( ) ), Is.False );
            Assert.That( _viewModel.DeleteCommand.CanExecute( new SecureString( ) ), Is.False );
            Assert.That( _viewModel.CopyCommand.CanExecute( new SecureString( ) ), Is.False );
        }

        [ Test ]
        public void TitleUpdatedByKeyUpdates( )
        {
            // Setup
            // Exercise
            _viewModel.Key = "abc";
            // Verify
            Assert.That( _viewModel.Title, Is.EqualTo( "abc*" ) );
        }

        [ Test ]
        public void GeneratedPasswordsSlotsInitiallyPresentWithoutContent( )
        {
            // Setup

            // Exercise
            // Verify
            Assert.That( _viewModel.Slots, Is.Not.Empty );
            Assert.That( _viewModel.Slots.Select( s => s.Generator ), Is.EquivalentTo( _editor.PasswordSlots ) );
            Assert.That( _viewModel.Slots.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            _viewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            // Verify
            Assert.That( _viewModel.Slots.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            _viewModel.Key = "abc";
            SecureString masterPassword = Util.Secure( "12345" );
            // Exercise
            _viewModel.UpdateMasterPassword( masterPassword );
            // Verify
            Assert.That(
                _viewModel.Slots.Select( s => s.Content == s.Generator.MakePassword( "abc", masterPassword ) ),
                Has.All.True );
        }
    }
}