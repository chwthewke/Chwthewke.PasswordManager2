using System;
using System.Windows.Media;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
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
            AddPassword( "abc", PasswordGenerators2.AlphaNumeric, 1, "12345".ToSecureString( ), string.Empty );
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
            AddPassword( "abc", PasswordGenerators2.AlphaNumeric, 1, "12345".ToSecureString( ), string.Empty );
            Guid masterPasswordGuid = PasswordRepository.LoadPassword( "abc" ).MasterPasswordId;
            // Exercise
            ViewModel.UpdateMasterPassword( "12345".ToSecureString( ) );
            // Verify
            Assert.That( ViewModel.ActualGuidColor, Is.EqualTo( GuidToColorConverter.Convert( masterPasswordGuid ) ) );
        }

        [ Test ]
        public void RequiredGuidColorIsSetAfterLoadingPassword( )
        {
            // Setup

            AddPassword( "abde", PasswordGenerators2.AlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            PasswordDigestDocument password = PasswordRepository.LoadPassword( "abde" );
            Guid expectedMasterPasswordId = password.MasterPasswordId;

            // Exercise
            ViewModel = ViewModelFactory.PasswordEditorFor( password );

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
            ViewModel.DerivedPasswords[ 0 ].IsSelected = true;

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
            AddPassword( "abc", PasswordGenerators2.Full, 1, "123".ToSecureString( ), string.Empty );

            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abc" ) );

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
            AddPassword( "abd", PasswordGenerators2.Full, 1, "123".ToSecureString( ), string.Empty );
            AddPassword( "abc", PasswordGenerators2.Full, 1, "123".ToSecureString( ), string.Empty );

            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abc" ) );

            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );
            // Exercise
            ViewModel.DeleteCommand.Execute( null );
            // Verify
            Assert.That( ViewModel.RequiredGuidColor, Is.EqualTo( Colors.Transparent ) );
            Assert.That( ViewModel.ActualGuidColor, Is.Not.EqualTo( Colors.Transparent ) );
        }
    }
}