using System;
using System.Windows.Media;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Test.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class GuidColorsTest : PasswordEditorTestBase
    {
        [ Test ]
        public void NoRequiredGuidColorWhenUnsaved( )
        {
            // Setup

            // Exercise
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
        }

        [ Test ]
        public void NoActualGuidColorWithEmptyMasterPassword( )
        {
            // Setup

            // Exercise
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
        }

        [ Test ]
        public void NoActualGuidColorWithUnknownMasterPassword( )
        {
            // Setup
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
        }

        [ Test ]
        public void NoActualGuidColorWithUnknownMasterPasswordAfter( )
        {
            // Setup
            Container.AddPassword( "abc", string.Empty, PasswordGenerators.AlphaNumeric, "12345".ToSecureString( ) );
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Exercise
            ViewModel.UpdateMasterPassword( "123456".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
        }

        [ Test ]
        public void ActualGuidColorIsSetWithKnownMasterPassword( )
        {
            // Setup
            Container.AddPassword( "abc", string.Empty, PasswordGenerators.AlphaNumeric, "12345".ToSecureString( ) );
            Guid masterPasswordGuid = PasswordStore.FindPasswordInfo( "abc" ).MasterPasswordId;
            // Exercise
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.ActualGuidColor, Is.EqualTo( GuidToColorConverter.Convert( masterPasswordGuid ) ) );
        }

        [ Test ]
        public void RequiredGuidColorIsSetAfterLoadingPassword( )
        {
            // Setup

            Container.AddPassword( "abde", "yadda yadda", PasswordGenerators.AlphaNumeric, "123".ToSecureString( ) );
            Guid expectedMasterPasswordId = PasswordStore.FindPasswordInfo( "abde" ).MasterPasswordId;

            // Exercise
            ViewModel.Key = "abde";
            ViewModel.LoadCommand.Execute( null );

            // Verify
            Assert.That( ViewModel.RequiredGuidColor,
                         Is.EqualTo( GuidToColorConverter.Convert( expectedMasterPasswordId ) ) );
        }

        [ Test ]
        public void GuidColorsAreSetAfterSavingPassword( )
        {
            // Setup
            ViewModel.Key = "abde";
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            ViewModel.Slots[ 0 ].IsSelected = true;

            Assert.That( ViewModel.SaveCommand.CanExecute( null ), Is.True );
            // Exercise
            ViewModel.SaveCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.Not.EqualTo( Colors.Transparent ) );
            Assert.That( ViewModel.ActualGuidColor, Is.EqualTo( ViewModel.RequiredGuidColor ) );
        }

        [ Test ]
        public void GuidColorsAreUnsetAfterDeletingPassword( )
        {
            // Setup
            Container.AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel.Key = "abc";
            ViewModel.LoadCommand.Execute( null );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
            Assert.That( ViewModel.ActualGuidColor, Is.EqualTo( Colors.Transparent ) );
        }

        [ Test ]
        public void ActualGuidColorIsKeptAfterDeletingPasswordIfStillPresent( )
        {
            // Setup
            Container.AddPassword( "abd", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            Container.AddPassword( "abc", string.Empty, PasswordGenerators.Full, "123".ToSecureString( ) );
            ViewModel.Key = "abc";
            ViewModel.LoadCommand.Execute( null );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
            Assert.That( ViewModel.ActualGuidColor, Is.Not.EqualTo( Colors.Transparent ) );
        }
    }
}