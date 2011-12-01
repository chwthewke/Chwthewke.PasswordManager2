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
    public class PasswordEditorModelWithoutPasswordTest
    {
        private IPasswordEditorModel _model;
        private IPasswordDerivationEngine _engine;
        private IPasswordRepository _passwordRepository;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators.Generators );

            _passwordRepository = new PasswordRepository( new InMemoryPasswordData( ) );

            IMasterPasswordMatcher masterPasswordMatcher = new MasterPasswordMatcher( _engine, _passwordRepository );

            _model = new PasswordEditorModel( _passwordRepository, _engine, masterPasswordMatcher, new StubTimeProvider( ), new NewPasswordDocument( ) );
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
                         Is.EquivalentTo( GeneratorGuids.Select( IsPasswordModel.Empty ).ToList( ) )
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
        public void ChangeKeyAndSelectGeneratorWithMasterPasswordMakesDirty( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            // Exercise
            _model.Key = "kolp";
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "kolp" ) );
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeKeyAndSelectGeneratorWithMasterPasswordThenUpdateDerivedMakesDirtyAndSaveable( )
        {
            // Set up
            _model.MasterPassword = "AAA".ToSecureString( );
            // Exercise
            _model.Key = "kolp";
            _model.UpdateDerivedPasswords( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "kolp" ) );
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ChangeKeyWithMasterPasswordMakesDirty( )
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
        public void SetKeyMasterPasswordAndIterationDoesNotGeneratePasswords( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.Iteration = 2;
            // Exercise
            var derivedPasswords = _model.DerivedPasswords.Select( m => m.DerivedPassword.Password );
            // Verify
            Assert.That( derivedPasswords, Has.All.EqualTo( string.Empty ) );
        }

        [ Test ]
        public void UpdateDerivedPasswordsWithKeyMasterPasswordAndIterationGeneratesPasswords( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.Iteration = 2;
            // Exercise
            _model.UpdateDerivedPasswords( );
            var derivedPasswords = _model.DerivedPasswords.Select( m => m.DerivedPassword.Password ).ToList( );
            // Verify
            Assert.That( derivedPasswords, Has.None.EqualTo( string.Empty ) );
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
            _model.UpdateDerivedPasswords( );
            // Exercise
            _model.Note = "A rather longer note.";
            // Verify
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ReloadWithoutBackEndChangeKeepsContentAndStateUnchanged( )
        {
            // Set up
            _model.Key = "Toto";
            _model.MasterPassword = "AAA".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            _model.Iteration = 4;
            _model.UpdateDerivedPasswords( );
            _model.Note = "A rather longer note.";

            // Exercise
            _model.Reload( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "Toto" ) );
            Assert.That( _model.MasterPassword.ConsumeBytes( Encoding.UTF8, b => Encoding.UTF8.GetString( b ) ),
                         Is.EqualTo( "AAA" ) );
            Assert.That( _model.SelectedPassword, Is.EqualTo( _model.DerivedPasswords.First( ) ) );
            Assert.That( _model.Iteration, Is.EqualTo( 4 ) );
            Assert.That( _model.Note, Is.EqualTo( "A rather longer note." ) );

            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.False );
        }

        [ Test ]
        public void ReloadWithBackEndChangeKeepsContentUnchanged( )
        {
            // Set up
            _model.Key = "Toto";
            _model.MasterPassword = "AAA".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            _model.Iteration = 4;
            _model.UpdateDerivedPasswords( );
            _model.Note = "A rather longer note.";

            _passwordRepository.SavePassword( new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "Toto",
                                                      Hash = new byte[ ] { 0x11, 0x22 },
                                                      Iteration = 1,
                                                      PasswordGenerator = PasswordGenerators.LegacyAlphaNumeric,
                                                      CreatedOn = new DateTime( 2011, 11, 3 ),
                                                      ModifiedOn = new DateTime( 2011, 11, 5 ),
                                                      MasterPasswordId = Guid.NewGuid( ),
                                                      Note = ""
                                                  } );

            // Exercise
            _model.Reload( );
            // Verify
            Assert.That( _model.Key, Is.EqualTo( "Toto" ) );
            Assert.That( _model.MasterPassword.ConsumeBytes( Encoding.UTF8, b => Encoding.UTF8.GetString( b ) ),
                         Is.EqualTo( "AAA" ) );
            Assert.That( _model.SelectedPassword, Is.EqualTo( _model.DerivedPasswords.First( ) ) );
            Assert.That( _model.Iteration, Is.EqualTo( 4 ) );
            Assert.That( _model.Note, Is.EqualTo( "A rather longer note." ) );

            Assert.That( _model.IsDirty, Is.True );
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