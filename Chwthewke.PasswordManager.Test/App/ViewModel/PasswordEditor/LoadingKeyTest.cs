using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class LoadingKeyTest : PasswordEditorTestBase
    {

        [ Test ]
        public void LoadEnablesWhenTypingStoredKey( )
        {
            // Setup

            Container.AddPassword( "abcd", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.True );
        }

        [ Test ]
        public void LoadDoesNotEnableUntilStoredKeyFullyTyped( )
        {
            // Setup
            Container.AddPassword( "abcd", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abc";
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadDoesNotEnableAfterStoredKeyTyped( )
        {
            // Setup
            Container.AddPassword( "abcd", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abcd";
            ViewModel.Key = "abcde";
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
        }

    }
}