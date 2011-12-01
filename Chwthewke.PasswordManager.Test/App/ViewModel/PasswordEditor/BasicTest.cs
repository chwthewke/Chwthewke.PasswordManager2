using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Engine;
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
            Assert.That( ViewModel.DerivedPasswords, Is.Not.Empty );
            Assert.That( ViewModel.DerivedPasswords.Select( s => s.Model.Generator ), Is.EquivalentTo( PasswordGenerators.Generators.Keys ) );
            Assert.That( ViewModel.DerivedPasswords.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void CloseSelfCommandRaisesCloseSelfRequest( )
        {
            // Setup
            IList<CloseEditorEventType> closeRequests = new List<CloseEditorEventType>( );
            ViewModel.CloseRequested += ( s, e ) => closeRequests.Add( e.Type );
            // Exercise
            ViewModel.CloseSelfCommand.Execute( null );
            // Verify
            Assert.That( closeRequests, Is.EquivalentTo( new[ ] { CloseEditorEventType.Self } ) );
        }

        [ Test ]
        public void CloseAllCommandRaisesCloseAllRequest( )
        {
            // Setup
            IList<CloseEditorEventType> closeRequests = new List<CloseEditorEventType>( );
            ViewModel.CloseRequested += ( s, e ) => closeRequests.Add( e.Type );
            // Exercise
            ViewModel.CloseAllCommand.Execute( null );
            // Verify
            Assert.That( closeRequests, Is.EquivalentTo( new[ ] { CloseEditorEventType.All } ) );
        }

        [ Test ]
        public void CloseAllButSelfCommandRaisesCloseAllButSelfRequest( )
        {
            // Setup
            IList<CloseEditorEventType> closeRequests = new List<CloseEditorEventType>( );
            ViewModel.CloseRequested += ( s, e ) => closeRequests.Add( e.Type );
            // Exercise
            ViewModel.CloseAllButSelfCommand.Execute( null );
            // Verify
            Assert.That( closeRequests, Is.EquivalentTo( new[ ] { CloseEditorEventType.AllButSelf } ) );
        }

        [ Test ]
        public void CloseToTheRightCommandRaisesCloseRightOfSelfRequest( )
        {
            // Setup
            IList<CloseEditorEventType> closeRequests = new List<CloseEditorEventType>( );
            ViewModel.CloseRequested += ( s, e ) => closeRequests.Add( e.Type );
            // Exercise
            ViewModel.CloseToTheRightCommand.Execute( null );
            // Verify
            Assert.That( closeRequests, Is.EquivalentTo( new[ ] { CloseEditorEventType.RightOfSelf } ) );
        }

        [ Test ]
        public void CloseInsecureCommandRaisesCloseInsecureRequest( )
        {
            // Setup
            IList<CloseEditorEventType> closeRequests = new List<CloseEditorEventType>( );
            ViewModel.CloseRequested += ( s, e ) => closeRequests.Add( e.Type );
            // Exercise
            ViewModel.CloseInsecureCommand.Execute( null );
            // Verify
            Assert.That( closeRequests, Is.EquivalentTo( new[ ] { CloseEditorEventType.Insecure } ) );
        }
    }
}