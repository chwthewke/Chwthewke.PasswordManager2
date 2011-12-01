using System;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;
using System.Linq;
using Chwthewke.PasswordManager.Test.Engine;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorUpdateTest
    {
        private IPasswordEditorModel _model;
        private IPasswordRepository _passwordRepository;
        private IPasswordDerivationEngine _engine;
        private StubTimeProvider _timeProvider;
        private PasswordDigestDocument _original;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators.Generators );
            _passwordRepository = new PasswordRepository( new InMemoryPasswordData( ) );

            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 3, PasswordGenerators.LegacyFull ) );

            _original = new PasswordDigestDocumentBuilder
                            {
                                Digest = digest.Digest,
                                CreatedOn = new DateTime( 2011, 11, 1 ),
                                ModifiedOn = new DateTime( 2011, 11, 3 ),
                                MasterPasswordId = Guid.NewGuid( ),
                                Note = "AB IJ"
                            };

            _passwordRepository.SavePassword( _original );

            _timeProvider = new StubTimeProvider { Now = new DateTime( 2011, 11, 16 ) };

            IMasterPasswordMatcher masterPasswordMatcher = new MasterPasswordMatcher( _engine, _passwordRepository );

            _model = new PasswordEditorModel( _passwordRepository, _engine, masterPasswordMatcher, _timeProvider, new BaselinePasswordDocument( _original ) );
        }

        [ Test ]
        public void SaveWhenNotSaveableDoesNotModifyCollection( )
        {
            // Set up

            _model.SelectedPassword = _model.DerivedPasswords.First( );

            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.False );
            Assert.That( _passwordRepository.LoadPassword( "abij" ), Is.EqualTo( _original ) );
        }

        [ Test ]
        public void SaveUpdatesPasswordInCollection( )
        {
            // Set up
            _model.MasterPassword = "12".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abij" ), Is.Not.EqualTo( _original ) );
        }

        [ Test ]
        public void ChangeMasterPasswordUpdatesMasterPasswordId( )
        {
            // Set up
            _model.MasterPassword = "123".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abij" ).MasterPasswordId,
                         Is.Not.EqualTo( _original.MasterPasswordId ) );
        }

        [ Test ]
        public void ChangePasswordGeneratorUpdatesGenerator( )
        {
            // Set up
            _model.MasterPassword = "1234".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            _model.SelectedPassword = _model.DerivedPasswords.First( p => p != _model.SelectedPassword );
            Guid expected = _model.SelectedPassword.Generator;
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abij" ).PasswordGenerator,
                         Is.EqualTo( expected ) );
        }

        [ Test ]
        public void ChangeIterationUpdatesIteration( )
        {
            // Set up
            _model.MasterPassword = "1234".ToSecureString( );
            _model.Iteration = 10;
            _model.UpdateDerivedPasswords( );
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abij" ).Iteration,
                         Is.EqualTo( 10 ) );
        }

        [ Test ]
        public void ChangeNoteUpdatesNote( )
        {
            // Set up
            _model.MasterPassword = "1234".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            const string aReallyDifferentNote = "A really different note.";
            _model.Note = aReallyDifferentNote;
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abij" ).Note,
                         Is.EqualTo( aReallyDifferentNote ) );
        }

        [ Test ]
        public void ChangeNoteOnlyUpdatesNote( )
        {
            // Set up
            const string aReallyDifferentNote = "A really different note.";
            _model.Note = aReallyDifferentNote;
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abij" ).Note,
                         Is.EqualTo( aReallyDifferentNote ) );
        }


        [ Test ]
        public void UpatedPasswordWasCreatedThenAndModifiedNow( )
        {
            // Set up
            _model.Iteration = 7;
            _model.MasterPassword = "1234".ToSecureString( );
            _model.UpdateDerivedPasswords( );
            DateTime now = new DateTime( 2011, 11, 7 );
            _timeProvider.Now = now;
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abij" );
            // Verify
            Assert.That( savedPassword.CreatedOn, Is.EqualTo( _original.CreatedOn ) );
            Assert.That( savedPassword.ModifiedOn, Is.EqualTo( now ) );
        }

        [Test]
        public void AfterSaveEditorIsNotDirtyAndKeyIsReadonly( )
        {
            // Set up
            _model.MasterPassword = "4321".ToSecureString( );
            _model.UpdateDerivedPasswords( );

            // Exercise
            _model.Save( );
            // Verify
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
            Assert.That( _model.IsKeyReadonly, Is.True );
        } 

    }
}