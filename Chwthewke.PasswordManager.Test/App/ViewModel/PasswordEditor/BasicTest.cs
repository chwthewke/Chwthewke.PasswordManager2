using System.Security;
using Autofac;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Modules;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class BasicTest : TestBase
    {

        [ Test ]
        public void InitialStateIsVoid( )
        {
            // Setup
            // Exercise
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( string.Empty ) );
            Assert.That( ViewModel.Title, Is.EqualTo( PasswordManager.App.ViewModel.PasswordEditorViewModel.NewTitle ) );
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
            Assert.That( ViewModel.Slots.Select( s => s.Generator ), Is.EquivalentTo( Editor.Generators ) );
            Assert.That( ViewModel.Slots.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

    }
}