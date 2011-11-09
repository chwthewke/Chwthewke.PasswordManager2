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
        private IPasswordEditorModelFactory _modelFactory;
        private IPasswordEditorModel _model;
        private IPasswordCollection _passwordCollection;
        private IPasswordDerivationEngine _engine;
        private StubTimeProvider _timeProvider;
        private PasswordDigestDocument _original;
        private InMemoryPasswordData _passwordData;

        [ SetUp ]
        public void SetUpModel( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators2.Generators );
            _passwordData = new InMemoryPasswordData( );
            _passwordCollection = new PasswordCollection( _passwordData );

            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 3, PasswordGenerators2.Full ) );

            _original = new PasswordDigestDocumentBuilder
                            {
                                Digest = digest.Digest,
                                CreatedOn = new DateTime( 2011, 11, 1 ),
                                ModifiedOn = new DateTime( 2011, 11, 3 ),
                                MasterPasswordId = Guid.NewGuid( ),
                                Note = "AB IJ"
                            };

            _passwordCollection.SavePassword( _original );

            _timeProvider = new StubTimeProvider( );

            _modelFactory = new PasswordEditorModelFactory( _passwordCollection, _engine, _timeProvider );
            _model = _modelFactory.CreateModel( _original );
        }

        [ Test ]
        public void DeletePasswordRemovesFromCollection( )
        {
            // Set up

            // Exercise
            _model.Delete( );
            // Verify
            Assert.That( _passwordCollection.LoadPasswords( ), Is.Empty );
        }

        [Test]
        public void DeletePasswordKeepsDeletedPasswordInData( )
        {
            // Set up

            // Exercise
            _model.Delete( );
            // Verify
            Assert.That( _passwordData.LoadPasswords( ), Has.Count.EqualTo( 1 ) );
            Assert.That( _passwordData.LoadPasswords( )[ 0 ].IsDeleted, Is.True );
        }

        [Test]
        public void DeletePasswordKeepsEditorFilledAndDirty( )
        {
            // Set up

            // Exercise
            _model.Delete( );
            // Verify
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