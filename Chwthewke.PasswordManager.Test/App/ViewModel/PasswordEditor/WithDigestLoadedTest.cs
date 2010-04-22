using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
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
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.LoadPasswordDigest( digest.Key );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Slots.First( s => s.Generator == PasswordGenerators.AlphaNumeric ).IsSelected );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.Title, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadDigestAllowsDeleteCommand( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.LoadPasswordDigest( digest.Key );
            // Verify
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.True );
        }

        [ Test ]
        public void DeletePasswordWhenCommandAvailable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            ViewModel.LoadPasswordDigest( digest.Key );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            StoreMock.Verify( store => store.Remove( "abde" ) );
        }

        [ Test ]
        public void LoadDigestCannotBeRepeated( )
        {
            // Setup
            StoreMock.Setup( sm => sm.Passwords ).Returns(
                new PasswordDigest[ ]
                    {
                        new PasswordDigestBuilder( )
                            .WithKey( "abde" )
                            .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                    }
                );
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.LoadPasswordDigest( digest.Key );
            // Verify
            Assert.That( ViewModel.LoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadDigestMakesDigestLoaded( )
        {
            // Setup

            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.LoadPasswordDigest( digest.Key );
            // Verify
            Assert.That( ViewModel.IsDigestLoaded, Is.True );
        }

        [ Test ]
        public void LoadDigestMakesKeyUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            ViewModel.LoadPasswordDigest( digest.Key );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadDigestMakesPasswordSlotSelectionUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.LoadPasswordDigest( digest.Key );
            // Verify
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }

        [ Test ]
        public void LoadDigestPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            ViewModel.LoadPasswordDigest( digest.Key );
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            // Exercise
            ViewModel.UpdateMasterPassword( Util.Secure( string.Empty ) );
            // Verify
            Assert.That( ViewModel.Slots.Any( slot => slot.IsSelected ) );
        }
    }
}