using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class KeyAndMasterPasswordTest : TestBase
    {
        [ Test ]
        public void TitleNotUpdatedByKeyInWhitespace( )
        {
            // Setup
            // Exercise
            ViewModel.Key = "  \t";
            // Verify
            Assert.That( ViewModel.Title, Is.EqualTo( PasswordManager.App.ViewModel.PasswordEditorViewModel.NewTitle ) );
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
        public void PasswordsAreNotGeneratedWithoutAKey( )
        {
            // Setup

            // Exercise
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
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
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            // Verify
            Assert.That( ViewModel.Slots.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }

        [ Test ]
        public void PasswordsAreGeneratedWithKeyAndMasterPassword( )
        {
            // Setup
            ViewModel.Key = "abc";
            SecureString masterPassword = Util.Secure( "12345" );
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
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That(
                ViewModel.Slots
                    .Select( s => s.Content == s.Generator.MakePassword( "abcd", Util.Secure( "12345" ) ) )
                    .ToList( ),
                Has.All.True );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
        }

        [ Test ]
        public void GeneratedPasswordsAreClearedOnKeyClear( )
        {
            // Setup
            ViewModel.Key = "abc";
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
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
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            // Exercise
            ViewModel.UpdateMasterPassword( Util.Secure( string.Empty ) );
            // Verify
            Assert.That( ViewModel.Slots.Select( s => s.Content ).ToList( ), Has.All.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }
    }
}