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
            Assert.That( ViewModel.Title, Is.EqualTo( PasswordEditorViewModel.NewTitle + "*" ) );
        }

        [ Test ]
        public void TitleUpdatedByKeyUpdates( )
        {
            // Setup
            // Exercise
            ViewModel.Key = "abc";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( "abc*" ) );
        }

        [ Test ]
        public void LongKeyIsTruncatedInTitle( )
        {
            // Setup

            // Exercise
            ViewModel.Key = "abcdefghij0123456789abcdefghij";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( "abcdefghij0123456789abcd...*" ) );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.DerivedPasswords.Select( s => s.Content ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void PasswordsAreNotGeneratedWithWhitespaceOnlyKey( )
        {
            // Setup
            ViewModel.Key = "  \t";
            // Exercise
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.DerivedPasswords.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.Iteration = 3;
            SecureString masterPassword = "12345".ToSecureString( );
            // Exercise
            ViewModel.UpdateMasterPassword( masterPassword );
            // Verify
            Assert.That(
                ViewModel.DerivedPasswords.Select( s => s.Content == DerivedPassword( s.Model.Generator, "abc", masterPassword, 3 ) ).ToList( ),
                Has.All.True );
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
                ViewModel.DerivedPasswords
                    .Select( s => s.Content == DerivedPassword( s.Model.Generator, "abcd", "12345".ToSecureString( ), 1 ) )
                    .ToList( ),
                Has.All.True );
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
            Assert.That( ViewModel.DerivedPasswords.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
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
            Assert.That( ViewModel.DerivedPasswords.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
        }
    }
}