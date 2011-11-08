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
    public class PasswordEditorModelWithPasswordTest
    {
        private IPasswordEditorModelFactory _modelFactory;
        private IPasswordEditorModel _model;
        private PasswordDigestDocument _original;
        private IPasswordCollection _passwordCollection;
        private IPasswordDerivationEngine _engine;

        [ SetUp ]
        public void SetUpModelFactory( )
        {
            _engine = new PasswordDerivationEngine( PasswordGenerators2.Generators );
            var digest = _engine.Derive( new PasswordRequest( "abij", "1234".ToSecureString( ), 3, PasswordGenerators2.Full ) );

            _original = new PasswordDigestDocumentBuilder
                            {
                                Digest = digest.Digest,
                                CreatedOn = new DateTime( 2011, 11, 1 ),
                                ModifiedOn = new DateTime( 2011, 11, 3 ),
                                MasterPasswordId = Guid.NewGuid( ),
                                Note = "AB IJ"
                            };

            _passwordCollection = new PasswordCollection( new InMemoryPasswordData( ) );
            _passwordCollection.SavePassword( _original );
            _modelFactory = new PasswordEditorModelFactory( _passwordCollection, _engine );
            _model = _modelFactory.CreateModel( _original );
        }

        [ Test ]
        public void InitiallyHasValuesFromLoadedPassword( )
        {
            // Set up

            // Exercise

            // Verify

            Assert.That( _model.Key, Is.EqualTo( _original.Key ) );
            Assert.That( _model.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( g => IsPasswordModel.Empty( g, 3 ) ) )
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
        public void SetMasterPasswordToActualChangesDerivedPasswordsAndMasterPasswordId( )
        {
            // Set up

            // Exercise
            _model.MasterPassword = "1234".ToSecureString( );
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
        [ Ignore ]
        public void SetMasterPasswordToOtherMakesEditorSaveable( )
        {
            // Set up

            // Exercise
            _model.MasterPassword = "123".ToSecureString( );
            // Verify
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( GeneratorGuids.Select( g => IsPasswordModel.For( g, "abij", "123".ToSecureString( ), 3 ) ) )
                             .Using( DerivedPasswordEquality ) );
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.IsDirty, Is.True );
            Assert.That( _model.CanSave, Is.True );
            Assert.That( _model.CanDelete, Is.True );
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