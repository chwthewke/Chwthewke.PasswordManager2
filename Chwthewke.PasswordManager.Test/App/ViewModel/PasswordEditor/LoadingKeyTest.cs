using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class LoadingKeyTest : TestWithStoreBase
    {

        [ Test ]
        public void LoadEnablesWhenTypingStoredKey( )
        {
            // Setup
            StoreMock.Setup( s => s.FindPasswordInfo( "abcd" ) )
                .Returns( new PasswordDigestBuilder( ).WithKey( "abcd" ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.LoadEnabled, Is.True );
        }

        [ Test ]
        public void LoadDoesNotEnableUntilStoredKeyFullyTyped( )
        {
            // Setup
            StoreMock.Setup( s => s.Passwords ).Returns( new PasswordDigest[ ]
                                                              { new PasswordDigestBuilder( ).WithKey( "abcd" ) } );
            // Exercise
            ViewModel.Key = "abc";
            // Verify
            Assert.That( ViewModel.LoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadDoesNotEnableAfterStoredKeyTyped( )
        {
            // Setup
            StoreMock.Setup( s => s.Passwords ).Returns( new PasswordDigest[ ]
                                                              { new PasswordDigestBuilder( ).WithKey( "abcd" ) } );
            // Exercise
            ViewModel.Key = "abcd";
            ViewModel.Key = "abcde";
            // Verify
            Assert.That( ViewModel.LoadEnabled, Is.False );
        }

    }
}