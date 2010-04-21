using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class WithDigestLoadedTest : TestWithStoreBase
    {
        [ Test ]
        public void LoadDigestSetsRelevantFields( )
        {
            // Setup
            // Exercise
            ViewModel.LoadPasswordDigest( new PasswordDigestBuilder( )
                                              .WithKey( "abde" )
                                              .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                                              .WithNote( "yadda yadda" ) );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Slots.First( s => s.Generator == PasswordGenerators.AlphaNumeric ).IsSelected );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.Title, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadDigestCannotBeRepeated( )
        {
            // Setup
            StoreMock.Setup( sm => sm.Passwords ).Returns(
                new PasswordDigest[ ] { new PasswordDigestBuilder( ).WithKey( "abde" ) }
                );
            // Exercise
            ViewModel.LoadPasswordDigest( new PasswordDigestBuilder( )
                                              .WithKey( "abde" ) );
            // Verify
            Assert.That( ViewModel.LoadEnabled, Is.False );
        }

        [Test]
        public void LoadDigestMakesKeyReadonly( )
        {
            // Setup

            // Exercise
            ViewModel.LoadPasswordDigest( new PasswordDigestBuilder( )
                                              .WithKey( "abde" ) );
            // Verify
            Assert.That( ViewModel.IsKeyReadOnly, Is.True );
        }
        [Test]
        public void LoadDigestMakesKeyUnmodifiable( )
        {
            // Setup
            ViewModel.LoadPasswordDigest( new PasswordDigestBuilder( )
                                              .WithKey( "abde" ) );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }
    }
}