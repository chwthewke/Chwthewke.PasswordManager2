using System;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.ViewModel.PasswordEditor
{
    [ TestFixture ]
    public class ReloadTest : PasswordEditorTestBase
    {
        [ Test ]
        public void ReloadPasswordUpdatesNote( )
        {
            // Setup
            var original = AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            bool noteUpdated = false;
            ViewModel.PropertyChanged += ( s, e ) => { noteUpdated |= e.PropertyName == "Note"; };

            PasswordRepository.UpdatePassword( original,
                                               new PasswordDigestDocumentBuilder
                                                   {
                                                       Digest = original.Digest,
                                                       Note = "yiddi yiddi",
                                                       CreatedOn = original.CreatedOn,
                                                       ModifiedOn = original.ModifiedOn.AddDays( 1 ),
                                                       MasterPasswordId = original.MasterPasswordId
                                                   } );
            // Exercise
            ViewModel.Reload( );
            // Verify
            Assert.That( ViewModel.Note, Is.EqualTo( "yiddi yiddi" ) );
            Assert.That( noteUpdated, Is.True );
        }

        [ Test ]
        public void ReloadPasswordUpdatesMasterPasswordIdAndExpectedColor( )
        {
            // Setup
            var original = AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            bool expecteColorUpdated = false;
            ViewModel.PropertyChanged += ( s, e ) => { expecteColorUpdated |= e.PropertyName == "RequiredGuidColor"; };

            PasswordDigestDocument updated = new PasswordDigestDocumentBuilder
                                                 {
                                                     Digest = original.Digest,
                                                     Note = original.Note,
                                                     CreatedOn = original.CreatedOn,
                                                     ModifiedOn = original.ModifiedOn.AddDays( 1 ),
                                                     MasterPasswordId = Guid.NewGuid( )
                                                 };
            PasswordRepository.UpdatePassword( original, updated );
            // Exercise
            ViewModel.Reload( );
            // Verify
            Assert.That( ViewModel.RequiredGuidColor,
                         Is.EqualTo( GuidToColorConverter.Convert( updated.MasterPasswordId ) ) );
            Assert.That( expecteColorUpdated, Is.True );
        }

        [ Test ]
        public void ReloadPasswordUpdatesActualGuidColor( )
        {
            // Setup
            var original = AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );
            ViewModel.UpdateMasterPassword( "123".ToSecureString( ) );

            bool guidColorUpdated = false;
            ViewModel.PropertyChanged += ( s, e ) => { guidColorUpdated |= e.PropertyName == "ActualGuidColor"; };

            PasswordDigestDocument updated = new PasswordDigestDocumentBuilder
                                                 {
                                                     Digest = original.Digest,
                                                     Note = original.Note,
                                                     CreatedOn = original.CreatedOn,
                                                     ModifiedOn = original.ModifiedOn.AddDays( 1 ),
                                                     MasterPasswordId = Guid.NewGuid( )
                                                 };

            PasswordRepository.UpdatePassword( original, updated );
            // Exercise
            ViewModel.Reload( );
            // Verify
            Assert.That( ViewModel.ActualGuidColor,
                         Is.EqualTo( GuidToColorConverter.Convert( updated.MasterPasswordId ) ) );
            Assert.That( guidColorUpdated, Is.True );
        }

        [ Test ]
        public void ReloadPasswordUpdateIteration( )
        {
            // Set up
            var original = AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            bool iterationUpdated = false;
            ViewModel.PropertyChanged += ( s, e ) => { iterationUpdated |= e.PropertyName == "Iteration"; };

            PasswordDigestDocument updated = new PasswordDigestDocumentBuilder
                                                 {
                                                     Digest = new PasswordDigest( original.Key, original.Hash, 2, original.PasswordGenerator ),
                                                     Note = original.Note,
                                                     CreatedOn = original.CreatedOn,
                                                     ModifiedOn = original.ModifiedOn.AddDays( 1 ),
                                                     MasterPasswordId = original.MasterPasswordId
                                                 };
            PasswordRepository.UpdatePassword( original, updated );

            // Exercise
            ViewModel.Reload( );

            // Verify
            Assert.That( ViewModel.Iteration, Is.EqualTo( 2 ) );
            Assert.That( iterationUpdated, Is.True );
        }

        [ Test ]
        public void ReloadPasswordUpdatesSelectedDerivedPassword( )
        {
            // Set up
            var original = AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            bool derivedPasswordUpdated = false;
            foreach ( var derivedPassword in ViewModel.DerivedPasswords )
            {
                derivedPassword.PropertyChanged += ( s, e ) => { if ( e.PropertyName == "IsSelected" ) derivedPasswordUpdated = true; };
            }

            PasswordDigestDocument updated = new PasswordDigestDocumentBuilder
                                                 {
                                                     Digest = new PasswordDigest( original.Key, original.Hash, 1, PasswordGenerators.LegacyFull ),
                                                     Note = original.Note,
                                                     CreatedOn = original.CreatedOn,
                                                     ModifiedOn = original.ModifiedOn.AddDays( 1 ),
                                                     MasterPasswordId = original.MasterPasswordId
                                                 };
            PasswordRepository.UpdatePassword( original, updated );

            // Exercise
            ViewModel.Reload( );
            // Verify
            Assert.That( ViewModel.DerivedPasswords.First( p => p.IsSelected ).Model.Generator,
                         Is.EqualTo( PasswordGenerators.LegacyFull ) );
            Assert.That( derivedPasswordUpdated, Is.True );
        }

        [ Test ]
        public void ReloadPasswordDoesNotUpdateWhenDirty( )
        {
            // Set up
            var original = AddPassword( "abde", PasswordGenerators.LegacyAlphaNumeric, 1, "123".ToSecureString( ), "yadda yadda" );
            ViewModel = ViewModelFactory.PasswordEditorFor( PasswordRepository.LoadPassword( "abde" ) );

            var updatedDigest =
                Engine.Derive( new PasswordRequest( original.Key, "123".ToSecureString( ), 1, PasswordGenerators.LegacyFull ) ).Digest;

            PasswordDigestDocument updated = new PasswordDigestDocumentBuilder
                                                 {
                                                     Digest = updatedDigest,
                                                     Note = original.Note,
                                                     CreatedOn = original.CreatedOn,
                                                     ModifiedOn = original.ModifiedOn.AddDays( 1 ),
                                                     MasterPasswordId = Guid.NewGuid( )
                                                 };
            PasswordRepository.UpdatePassword( original, updated );

            ViewModel.Iteration = 3;
            // Exercise
            ViewModel.Reload( );
            // Verify
            Assert.That( ViewModel.Iteration, Is.EqualTo( 3 ) );
            Assert.That( ViewModel.DerivedPasswords.First( p => p.IsSelected ).Model.Generator, Is.EqualTo( PasswordGenerators.LegacyAlphaNumeric ) );
        }
    }
}