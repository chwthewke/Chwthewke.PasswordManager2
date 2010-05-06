using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class WithDigestLoadedTest : PasswordEditorTestBase
    {
        [ Test ]
        public void LoadPasswordSetsRelevantFields( )
        {
            // Setup

            Container.AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );

            // Exercise
            ViewModel.Key = "abde";
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
            Container.AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );

            // Exercise
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.DeleteCommand.CanExecute( null ), Is.True );
        }

        [ Test ]
        public void LoadPasswordCannotBeRepeated( )
        {
            // Setup
            Container.AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
        }

        [ Test ]
        public void LoadPasswordMakesPasswordLoaded( )
        {
            // Setup

            Container.AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.IsKeyReadonly, Is.True );
        }

        [ Test ]
        public void LoadPasswordMakesKeyUnmodifiable( )
        {
            // Setup
            Container.AddPassword( "abde", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );
            // Exercise
            ViewModel.Key = "abcd";
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
        }

        [ Test ]
        public void LoadPasswordKeepsPasswordSlotSelectionPossible( )
        {
            // Setup
            Container.AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            // Exercise
            ViewModel.Key = "abde";
            ViewModel.UpdateMasterPassword( "1234".ToSecureString( ) );
            ViewModel.LoadCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
        }

        [ Test ]
        public void LoadPasswordPreventsPasswordSlotSelectionFromClear( )
        {
            // Setup
            Container.AddPassword( "abde", string.Empty, PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateMasterPassword( string.Empty.ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.Slots.Any( slot => slot.IsSelected ) );
        }

        [ Test ]
        public void DeletePasswordWhenCommandAvailable( )
        {
            // Setup
            Container.AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( PasswordStore.FindPasswordInfo( "abde" ), Is.Null );
        }

        [ Test ]
        public void DeletePasswordRaisesStoreModified( )
        {
            // Setup
            Container.AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );

            bool storeModifiedRaised = false;
            ViewModel.StoreModified += ( s, e ) => { storeModifiedRaised = true; };
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( storeModifiedRaised, Is.True );
        }

        [ Test ]
        public void DeletePasswordDoesNotResetEditor( )
        {
            // Setup
            Container.AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            ViewModel.Key = "abde";
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            ViewModel.LoadCommand.Execute( null );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.Key, Is.EqualTo( "abde" ) );
            Assert.That( ViewModel.Note, Is.EqualTo( "yadda yadda" ) );
            Assert.That( ViewModel.CanSelectPasswordSlot, Is.True );
            Assert.That( ViewModel.Slots.First( slot => slot.IsSelected ).Generator,
                         Is.EqualTo( PasswordGenerators.AlphaNumeric ) );
            Assert.That( ViewModel.IsLoadEnabled, Is.False );
            Assert.That( ViewModel.IsKeyReadonly, Is.False );
        }
    }
}