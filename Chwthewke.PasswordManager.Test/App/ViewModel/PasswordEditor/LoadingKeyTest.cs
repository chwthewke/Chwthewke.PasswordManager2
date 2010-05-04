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

            AddPassword( "abcd", string.Empty, PasswordGenerators.Full, Util.Secure( "123" ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.True );
        }

        [ Test ]
        public void LoadDoesNotEnableUntilStoredKeyFullyTyped( )
        {
            // Setup
            AddPassword( "abcd", string.Empty, PasswordGenerators.Full, Util.Secure( "123" ) );
            // Exercise
            ViewModel.Key = "abc";
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadDoesNotEnableAfterStoredKeyTyped( )
        {
            // Setup
            AddPassword( "abcd", string.Empty, PasswordGenerators.Full, Util.Secure( "123" ) );
            // Exercise
            ViewModel.Key = "abcd";
            ViewModel.Key = "abcde";
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
        }

    }
}