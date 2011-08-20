using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class KeyAndMasterPasswordTest : PasswordEditorTestBase
    {
        [ Test ]
        public void TitleNotUsingKeyInWhitespace( )
        {
            // Setup
            // Exercise
            ViewModel.Key = "  \t";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( PasswordEditorViewModel.NewTitle ) );
        }

        [ Test ]
        public void TitleDirtiedByNoteUpdate( )
        {
            // Setup
            // Exercise
            ViewModel.Note = "yiddi yoddo";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( PasswordEditorViewModel.NewTitle ) );
        }

        [ Test ]
        public void TitleUpdatedByKeyUpdates( )
        {
            // Setup
            // Exercise
            ViewModel.Key = "abc";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( "abc" ) );
        }

        [ Test ]
        public void LongKeyIsTruncatedInTitle( )
        {
            // Setup
            
            // Exercise
            ViewModel.Key = "abcdefghij0123456789abcdefghij";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( "abcdefghij0123456789abcd..." ) );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.Slots.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithWhitespaceOnlyKey( )
        {
            // Setup
            ViewModel.Key = "  \t";
            // Exercise
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.Slots.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }

        [ Test ]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            ViewModel.Key = "abc";
            SecureString masterPassword = "12345".ToSecureString( );
            // Exercise
            ViewModel.UpdateMasterPassword( masterPassword );
            // Verify
            Assert.That(
                ViewModel.Slots.Select( s => s.Content == s.Generator.MakePassword( "abc", masterPassword ) ).ToList( ),
                Has.All.True );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
        }

        [ Test ]
        public void GeneratedPasswordsAreUpdatedOnKeyUpdate( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That(
                ViewModel.Slots
                    .Select( s => s.Content == s.Generator.MakePassword( "abcd", "12345".ToSecureString( ) ) )
                    .ToList( ),
                Has.All.True );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
        }

        [ Test ]
        public void GeneratedPasswordsAreClearedOnKeyClear( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            ViewModel.Key = string.Empty;
            // Verify
            Assert.That( ViewModel.Slots.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }

        [ Test ]
        public void GeneratedPasswordsAreClearedOnMasterPasswordClear( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateMasterPassword( string.Empty.ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.Slots.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }
    }
}