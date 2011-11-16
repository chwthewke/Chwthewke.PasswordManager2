using System;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorDeleteTest
    {
        private IPasswordEditorModel _model;
        private IPasswordRepository _passwordRepository;
        private IPasswordDerivationEngine _engine;
        private StubTimeProvider _timeProvider;
        private PasswordDigestDocument _original;
        private InMemoryPasswordData _passwordData;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators2.Generators );
            _passwordData = new InMemoryPasswordData( );
            _passwordRepository = new PasswordRepository( _passwordData );

            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 3, PasswordGenerators2.Full ) );

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

            IMasterPasswordMatcher masterPasswordMatcher = new MasterPasswordMatcher2( _engine, _passwordRepository );

            _model = new PasswordEditorModel( _passwordRepository, _engine, masterPasswordMatcher, _timeProvider, _original );
        }

        [ Test ]
        public void DeletePasswordRemovesFromCollection( )
        {
            // Set up

            // Exercise
            var deleted = _model.Delete( );
            // Verify
            Assert.That( deleted, Is.True );
            Assert.That( _passwordRepository.LoadPasswords( ), Is.Empty );
        }


        [ Test ]
        public void DeletePasswordKeepsDeletedPasswordInData( )
        {
            // Set up
            // Exercise
            var deleted = _model.Delete( );
            // Verify
            Assert.That( deleted, Is.True );
            Assert.That( _passwordData.LoadPasswords( ), Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordData.LoadPasswords( )[ 0 ].IsDeleted, Is.True );
        }

        [ Test ]
        public void DeletePasswordModifiedEarlierReallyDeletesIt( )
        {
            // Set up
            _timeProvider.Now = new DateTime( 2011, 11, 5 );
            _passwordRepository.UpdatePassword( _original, new PasswordDigestDocumentBuilder
                                                               {
                                                                   Digest = _original.Digest,
                                                                   CreatedOn = _original.CreatedOn,
                                                                   ModifiedOn = new DateTime( 2011, 11, 4 ),
                                                                   Note = "I changed the note.",
                                                                   MasterPasswordId = _original.MasterPasswordId
                                                               } );

            // Exercise
            var deleted = _model.Delete( );
            // Verify
            Assert.That( deleted, Is.True );
            Assert.That( _passwordRepository.LoadPasswords( ), Is.Empty );
            Assert.That( _passwordData.LoadPasswords( ), Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordData.LoadPasswords( )[ 0 ].IsDeleted, Is.True );
        }

        [ Test ]
        public void DeletePasswordModifiedLaterLeavesIt( )
        {
            // Set up
            _timeProvider.Now = new DateTime( 2011, 11, 5 );
            PasswordDigestDocument updated = new PasswordDigestDocumentBuilder
                                                 {
                                                     Digest = _original.Digest,
                                                     CreatedOn = _original.CreatedOn,
                                                     ModifiedOn = new DateTime( 2011, 11, 6 ),
                                                     Note = "I changed the note.",
                                                     MasterPasswordId = _original.MasterPasswordId
                                                 };
            _passwordRepository.UpdatePassword( _original, updated );

            // Exercise
            var deleted = _model.Delete( );
            // Verify
            Assert.That( deleted, Is.False );
            Assert.That( _passwordRepository.LoadPasswords( ), Is.EquivalentTo( new[ ] { updated } ) );
            Assert.That( _passwordData.LoadPasswords( ), Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordData.LoadPasswords( )[ 0 ].IsDeleted, Is.False );
        }

        [ Test ]
        public void DeletePasswordKeepsEditorFilledAndDirty( )
        {
            // Set up

            // Exercise
            var deleted = _model.Delete( );
            // Verify
            Assert.That( deleted, Is.True );
            Assert.That( _model.Key, Is.EqualTo( "abij" ) );
            Assert.That( _model.SelectedPassword.Generator, Is.EqualTo( PasswordGenerators2.Full ) );
            Assert.That( _model.Iteration, Is.EqualTo( 3 ) );
            Assert.That( _model.Note, Is.EqualTo( "AB IJ" ) );

            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.False );
        }
    }
}