using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorModelWithPasswordTest
    {
        private IPasswordEditorModel _model;
        private PasswordDigestDocument _original;
        private IPasswordDerivationEngine _engine;
        private IPasswordRepository _passwordRepository;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators.Generators );
            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 3, PasswordGenerators.LegacyFull ) );

            _original = new PasswordDigestDocumentBuilder
                            {
                                Digest = digest.Digest,
                                CreatedOn = new DateTime( 2011, 11, 1 ),
                                ModifiedOn = new DateTime( 2011, 11, 3 ),
                                MasterPasswordId = Guid.NewGuid( ),
                                Note = "AB IJ"
                            };

            _passwordRepository = new PasswordRepository( new InMemoryPasswordData( ) );
            _passwordRepository.SavePassword( _original );

            IMasterPasswordMatcher masterPasswordMatcher = new MasterPasswordMatcher( _engine, _passwordRepository );

            _model = new PasswordEditorModel( _passwordRepository, _engine, masterPasswordMatcher, new StubTimeProvider( ),
                                              new BaselinePasswordDocument( _original ) );
        }

        [ Test ]
        public void InitiallyHasValuesFromLoadedPassword( )
        {
            // Set up

            // Exercise

            // Verify

            Assert.That( _model.Key, Is.EqualTo( _original.Key ) );
            Assert.That( _model.IsKeyReadonly, Is.True );
            Assert.That( _model.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _model.Iteration, Is.EqualTo( 3 ) );
            Assert.That( _model.Note, Is.EqualTo( _original.Note ) );
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( IsPasswordModel.Empty ) )
                             .Using( DerivedPasswordEquality ) );
            Assert.That( _model.SelectedPassword,
                         Is.SameAs( _model.DerivedPasswords.Single( dp => dp.Generator == _original.PasswordGenerator ) ) );
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.ExpectedMasterPasswordId, Is.EqualTo( _original.MasterPasswordId ) );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ChangeKeyHasNoEffect( )
        {
            // Set up

            // Exercise
            _model.Key = "kolp";
            // Verify
            Assert.That( _model.Key, Is.EqualTo( _original.Key ) );
        }

        [ Test ]
        public void SetMasterPasswordAndUpdateDerivedPasswordsToActualChangesDerivedPasswordsAndMasterPasswordId( )
        {
            // Set up

            // Exercise
            _model.MasterPassword = "1234".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Verify
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( g => IsPasswordModel.For( g, "abij", "1234".ToSecureString( ), 3 ) ) )
                             .Using( DerivedPasswordEquality ) );
            Assert.That( _model.MasterPasswordId, Is.EqualTo( _original.MasterPasswordId ) );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void SetMasterPasswordWithoutUpdateChangesNothing( )
        {
            // Set up

            // Exercise
            _model.MasterPassword = "123".ToSecureString( );
            // Verify
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( IsPasswordModel.Empty ) )
                             .Using( DerivedPasswordEquality ) );
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
        }


        [ Test ]
        public void SetMasterPasswordAndUpdateDerivedPasswordsToOtherMakesEditorSaveableButNotDirty( )
        {
            // Set up

            // Exercise
            _model.MasterPassword = "123".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Verify
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( g => IsPasswordModel.For( g, "abij", "123".ToSecureString( ), 3 ) ) )
                             .Using( DerivedPasswordEquality ) );
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }


        [ Test ]
        public void ChangeGeneratorMakesEditorDirty( )
        {
            // Set up

            // Exercise
            _model.SelectedPassword = _model.DerivedPasswords.First( p => p != _model.SelectedPassword );
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ChangeGeneratorWithMasterPasswordMakesEditorDirtyAndSaveable( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Exercise
            _model.SelectedPassword = _model.DerivedPasswords.First( p => p != _model.SelectedPassword );
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ChangeIterationsMakesEditorDirty( )
        {
            // Set up
            // Exercise
            _model.Iteration = 2;
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ChangeIterationsWithMasterPasswordMakesEditorDirtyAndSaveable( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Exercise
            _model.Iteration = 2;
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ChangeOnlyNoteMakesSaveable( )
        {
            // Set up

            // Exercise
            _model.Note = "A rather longer note.";
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }


        [ Test ]
        public void ChangeNoteAndIterationMakesDirtyButNotSaveable( )
        {
            // Set up

            // Exercise
            _model.Note = "A rather longer note.";
            _model.Iteration = 2;
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ChangeNoteWithMasterPasswordMakesSaveable( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Exercise
            _model.Note = "A rather longer note.";
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ReloadWithoutBackEndChangeKeepsContentAndStateUnchanged( )
        {
            // Set up
            _model.MasterPassword = "4321".ToSecureString( );
            _model.UpdateDerivedPasswords( );

            // Exercise
            _model.Reload( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "abij" ) );
            Assert.That( _model.MasterPassword.ConsumeBytes( Encoding.UTF8, b => Encoding.UTF8.GetString( b ) ),
                         Is.EqualTo( "4321" ) );
            Assert.That( _model.SelectedPassword,
                         Is.EqualTo( _model.DerivedPasswords.First( p => p.Generator == PasswordGenerators.LegacyFull ) ) );
            Assert.That( _model.Iteration, Is.EqualTo( 3 ) );
            Assert.That( _model.Note, Is.EqualTo( "AB IJ" ) );

            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ReloadWithBackEndChangeWhileDirtyKeepsContentUnchanged( )
        {
            // Set up
            _model.Iteration = 2;
            _model.MasterPassword = "4321".ToSecureString( );
            _model.UpdateDerivedPasswords( );

            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 5, PasswordGenerators.LegacyAlphaNumeric ) );

            var updated = new PasswordDigestDocumentBuilder
                              {
                                  Digest = digest.Digest,
                                  CreatedOn = new DateTime( 2011, 11, 2 ),
                                  ModifiedOn = new DateTime( 2011, 11, 5 ),
                                  MasterPasswordId = Guid.NewGuid( ),
                                  Note = "AB IJ K"
                              };


            Assert.That( _passwordRepository.SavePassword( updated ), Is.True );

            // Exercise
            _model.Reload( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "abij" ) );
            Assert.That( _model.MasterPassword.ConsumeBytes( Encoding.UTF8, b => Encoding.UTF8.GetString( b ) ),
                         Is.EqualTo( "4321" ) );
            Assert.That( _model.SelectedPassword,
                         Is.EqualTo( _model.DerivedPasswords.First( p => p.Generator == PasswordGenerators.LegacyFull ) ) );
            Assert.That( _model.Iteration, Is.EqualTo( 2 ) );
            Assert.That( _model.Note, Is.EqualTo( "AB IJ" ) );

            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }

        [ Test ]
        public void ReloadWithBackEndChangeWhileNotDirtyUpdatesContent( )
        {
            // Set up
            _model.MasterPassword = "4321".ToSecureString( );
            _model.UpdateDerivedPasswords( );

            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 5, PasswordGenerators.LegacyAlphaNumeric ) );

            var updated = new PasswordDigestDocumentBuilder
                              {
                                  Digest = digest.Digest,
                                  CreatedOn = new DateTime( 2011, 11, 2 ),
                                  ModifiedOn = new DateTime( 2011, 11, 5 ),
                                  MasterPasswordId = Guid.NewGuid( ),
                                  Note = "AB IJ K"
                              };


            Assert.That( _passwordRepository.SavePassword( updated ), Is.True );


            // Exercise
            _model.Reload( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "abij" ) );
            Assert.That( _model.MasterPassword.ConsumeBytes( Encoding.UTF8, b => Encoding.UTF8.GetString( b ) ),
                         Is.EqualTo( "4321" ) );
            Assert.That( _model.SelectedPassword,
                         Is.EqualTo( _model.DerivedPasswords.First( p => p.Generator == PasswordGenerators.LegacyAlphaNumeric ) ) );
            Assert.That( _model.Iteration, Is.EqualTo( 5 ) );
            Assert.That( _model.Note, Is.EqualTo( "AB IJ K" ) );

            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
        }


        private static IEnumerable<Guid> GeneratorGuids
        {
            get { return PasswordGenerators.Generators.Keys; }
        }

        private static DerivedPasswordEqualityComparer DerivedPasswordEquality
        {
            get { return new DerivedPasswordEqualityComparer( ); }
        }
    }
}