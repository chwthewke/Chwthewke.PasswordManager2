using System;
using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorModelWithoutPasswordTest
    {
        private IPasswordEditorModel _model;
        private IPasswordDerivationEngine _engine;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators2.Generators );

            PasswordRepository passwordRepository = new PasswordRepository( new InMemoryPasswordData( ) );

            IMasterPasswordMatcher masterPasswordMatcher = new MasterPasswordMatcher2( _engine, passwordRepository );

            _model = new PasswordEditorModel( passwordRepository, _engine, masterPasswordMatcher, new StubTimeProvider(  ) );
        }

        [ Test ]
        public void InitiallyHasDefaultValues( )
        {
            // Set up

            // Exercise

            // Verify

            Assert.That( _model.Key, Is.EqualTo( string.Empty ) );
            Assert.That( _model.IsKeyReadonly, Is.False );
            Assert.That( _model.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _model.Iteration, Is.EqualTo( 1 ) );
            Assert.That( _model.Note, Is.EqualTo( string.Empty ) );
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( IsPasswordModel.Empty ) )
                             .Using( DerivedPasswordEquality ) );
            Assert.That( _model.SelectedPassword, Is.Null );
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.ExpectedMasterPasswordId, Is.Null );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeKeyMakesDirty( )
        {
            // Set up

            // Exercise
            _model.Key = "kolp";
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "kolp" ) );
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeKeyAndSelectGeneratorWithMasterPasswodMakesDirtyAndSaveable( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            // Exercise
            _model.Key = "kolp";
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "kolp" ) );
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeKeyWithMasterPasswodMakesDirty( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            // Exercise
            _model.Key = "kolp";
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "kolp" ) );
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeMasterPasswordOnlyMakesEditorNeitherSaveableNorDirty( )
        {
            // Set up

            // Exercise
            _model.MasterPassword = "123".ToSecureString( );
            // Verify
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
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
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeGeneratorWithMasterPasswordMakesEditorDirty( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            // Exercise
            _model.SelectedPassword = _model.DerivedPasswords.First( p => p != _model.SelectedPassword );
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
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
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeIterationsWithMasterPasswordMakesEditorDirty( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            // Exercise
            _model.Iteration = 2;
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeOnlyNoteMakesEditorDirty( )
        {
            // Set up

            // Exercise
            _model.Note = "A rather longer note.";
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
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
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeNoteWithKeyMasterPasswordAndGeneratorMakesSaveable( )
        {
            // Set up
            _model.Key = "Toto";
            _model.MasterPassword = "AAA".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Exercise
            _model.Note = "A rather longer note.";
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.False );
        }

        private static IEnumerable<Guid> GeneratorGuids
        {
            get { return PasswordGenerators2.Generators.Keys; }
        }

        private static DerivedPasswordEqualityComparer DerivedPasswordEquality
        {
            get { return new DerivedPasswordEqualityComparer( ); }
        }
    }
}