using System;
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
        public void LoadPasswordSetsRelevantFields( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Slots.First( s => s.Generator == PasswordGenerators.AlphaNumeric ).IsSelected );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.Title, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordAllowsDeleteCommand( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.True );
        }

        [ Test ]
        public void LoadPasswordCannotBeRepeated( )
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
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordLoaded( )
        {
            // Setup

            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.Full.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordSlotSelectionUnmodifiable( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            // Exercise
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
        }

        [ Test ]
        public void LoadPasswordPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            ViewModel.UpdateMasterPassword( Util.Secure( "12345" ) );
            // Exercise
            ViewModel.UpdateMasterPassword( Util.Secure( string.Empty ) );
            // Verify
            Assert.That( ViewModel.Slots.Any( slot => slot.IsSelected ) );
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
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            StoreMock.Verify( store => store.Remove( "abde" ) );
        }

        [ Test ]
        public void DeletePasswordResetsEditor( )
        {
            // Setup
            PasswordDigest digest = new PasswordDigestBuilder( )
                .WithKey( "abde" )
                .WithGeneratorId( PasswordGenerators.AlphaNumeric.Id )
                .WithNote( "yadda yadda" );
            StoreMock.Setup( store => store.FindPasswordInfo( digest.Key ) ).Returns( digest );
            ViewModel.Key = digest.Key;
            ViewModel.LoadCommand.Execute( null );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( string.Empty ) );
            Assert.That( ViewModel.Note, Is.EqualTo( string.Empty ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.False );
            Assert.That( ViewModel.Slots.All( slot => !slot.IsSelected ) );
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
            Assert.That( ViewModel.IsKeyReadonly, Is.False );
        }
    }
}