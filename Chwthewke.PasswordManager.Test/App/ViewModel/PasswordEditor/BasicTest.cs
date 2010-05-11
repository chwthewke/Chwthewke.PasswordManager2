using System.Linq;
using Chwthewke.PasswordManager.App.ViewModel;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class BasicTest : PasswordEditorTestBase
    {
        [ Test ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( string.Empty ) );
            Assert.That( ViewModel.Title, Is.EqualTo( PasswordEditorViewModel.NewTitle ) );
            Assert.That( ViewModel.Note, Is.EqualTo( string.Empty ) );
            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.False );
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.False );
            Assert.That( ViewModel.CopyCommand.CanExecute( null ), Is.False );
        }


        [ Test ]
        public void GeneratedPasswordsSlotsInitiallyPresentWithoutContent( )
        {
            // Setup

            // Exercise
            // Verify
            Assert.That( ViewModel.Slots, Is.Not.Empty );
            Assert.That( ViewModel.Slots.Select( s => s.Generator ), Is.EquivalentTo( Controller.Generators ) );
            Assert.That( ViewModel.Slots.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void CloseCommandRaisesCloseRequested( )
        {
            // Setup
            bool closeRequested = false;
            ViewModel.CloseRequested += ( s, e ) => { closeRequested = true; };
            // Exercise
            ViewModel.CloseCommand.Execute( null );
            // Verify
            Assert.That( closeRequested, Is.True );
        }
    }
}