using System.Security;
using Chwthewke.PasswordManager.App.ViewModel;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor2
{
    [ TestFixture ]
    public class KeyAndMasterPasswordTest : PasswordEditorTestBase
    {
        [ Test ]
        public void TitleNotUsingKeyInWhitespace( )
        {
            // Setup
            // Exercise
            App.ViewModel.Key = "  \t";
            // Verify
            Assert.That( App.ViewModel.Title, Is.EqualTo( PasswordEditorViewModel.NewTitle ) );
        }

        [ Test ]
        public void TitleDirtiedByNoteUpdate( )
        {
            // Setup
            // Exercise
            App.ViewModel.Note = "yiddi yoddo";
            // Verify
            Assert.That( App.ViewModel.Title, Is.EqualTo( PasswordEditorViewModel.NewTitle ) );
        }

        [ Test ]
        public void TitleUpdatedByKeyUpdates( )
        {
            // Setup
            // Exercise
            App.ViewModel.Key = "abc";
            // Verify
            Assert.That( App.ViewModel.Title, Is.EqualTo( "abc" ) );
        }

        [ Test ]
        public void LongKeyIsTruncatedInTitle( )
        {
            // Setup

            // Exercise
            App.ViewModel.Key = "abcdefghij0123456789abcdefghij";
            // Verify
            Assert.That( App.ViewModel.Title, Is.EqualTo( "abcdefghij0123456789abcd..." ) );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            App.ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( App.ViewModel.DerivedPasswords.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithWhitespaceOnlyKey( )
        {
            // Setup
            App.ViewModel.Key = "  \t";
            // Exercise
            App.ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( App.ViewModel.DerivedPasswords.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            App.ViewModel.Key = "abc";
            SecureString masterPassword = "12345".ToSecureString( );
            // Exercise
            App.ViewModel.UpdateMasterPassword( masterPassword );
            // Verify
            Assert.That(
                App.ViewModel.DerivedPasswords.Select( s => s.Content == s.Generator.MakePassword( "abc", masterPassword ) ).ToList( ),
                Has.All.True );
        }

        [ Test ]
        public void GeneratedPasswordsAreUpdatedOnKeyUpdate( )
        {
            // Setup
            App.ViewModel.Key = "abc";
            App.ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            App.ViewModel.Key = "abcd";
            // Verify
            Assert.That(
                App.ViewModel.DerivedPasswords
                    .Select( s => s.Content == s.Generator.MakePassword( "abcd", "12345".ToSecureString( ) ) )
                    .ToList( ),
                Has.All.True );
        }

        [ Test ]
        public void GeneratedPasswordsAreClearedOnKeyClear( )
        {
            // Setup
            App.ViewModel.Key = "abc";
            App.ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            App.ViewModel.Key = string.Empty;
            // Verify
            Assert.That( App.ViewModel.DerivedPasswords.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void GeneratedPasswordsAreClearedOnMasterPasswordClear( )
        {
            // Setup
            App.ViewModel.Key = "abc";
            App.ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            App.ViewModel.UpdateMasterPassword( string.Empty.ToSecureString( ) );
            // Verify
            Assert.That( App.ViewModel.DerivedPasswords.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
        }
    }
}