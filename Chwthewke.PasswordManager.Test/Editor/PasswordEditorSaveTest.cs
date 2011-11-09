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
    public class PasswordEditorSaveTest
    {
        private IPasswordEditorModelFactory _modelFactory;
        private IPasswordEditorModel _model;
        private IPasswordRepository _passwordRepository;
        private IPasswordDerivationEngine _engine;
        private StubTimeProvider _timeProvider;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators2.Generators );

            _passwordRepository = new PasswordRepository( new InMemoryPasswordData( ) );

            _timeProvider = new StubTimeProvider( );

            _modelFactory = new PasswordEditorModelFactory( _passwordRepository, _engine, _timeProvider );
            _model = _modelFactory.CreatePrisineModel( );
        }

        [ Test ]
        public void SaveWhenNotSaveableDoesNotModifyCollection( )
        {
            // Set up
            _model.Key = "abcd";
            _model.SelectedPassword = _model.DerivedPasswords.First( );

            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.False );
            Assert.That( _passwordRepository.LoadPasswords( ), Is.Empty );
        }

        [ Test ]
        public void SaveAddsPasswordToCollection( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Exercise
            var saved = _model.Save( );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPasswords( ), Has.Count.EqualTo( 1 ) );
        }

        [ Test ]
        public void SavedPasswordHasSameKey( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( savedPassword.Key, Is.EqualTo( "abcd" ) );
        }

        [ Test ]
        public void SavedPasswordHasSameSelectedPassword( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( savedPassword.PasswordGenerator, Is.EqualTo( _model.SelectedPassword.Generator ) );
        }

        [ Test ]
        public void SavedPasswordHasSameIteration( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            _model.Iteration = 13;
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( savedPassword.Iteration, Is.EqualTo( 13 ) );
        }

        [ Test ]
        public void SavedPasswordHasNonDefaultMasterPasswordId( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( savedPassword.MasterPasswordId, Is.Not.EqualTo( default( Guid ) ) );
        }

        [ Test ]
        public void SavedPasswordHasSameNote( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            const string myNote = "My Note.";
            _model.Note = myNote;
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( savedPassword.Note, Is.EqualTo( myNote ) );
        }

        [ Test ]
        public void SavedPasswordWasCreatedAndModifiedNow( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            DateTime now = new DateTime( 2011, 11, 7 );
            _timeProvider.Now = now;
            // Exercise
            _model.Save( );
            var savedPassword = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( savedPassword.CreatedOn, Is.EqualTo( now ) );
            Assert.That( savedPassword.ModifiedOn, Is.EqualTo( now ) );
        }

        [ Test ]
        public void AfterSaveEditorIsNotDirtyAndKeyIsReadonly( )
        {
            // Set up
            _model.Key = "abcd";
            _model.MasterPassword = "1234".ToSecureString( );
            _model.SelectedPassword = _model.DerivedPasswords.First( );
            
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