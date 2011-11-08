using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Storage;
using NUnit.Framework;
using NUnit.Framework.Constraints;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Editor
{
    [ TestFixture ]
    public class PasswordEditorModelWithPasswordTest
    {
        private IPasswordEditorModelFactory _modelFactory;
        private IPasswordEditorModel _model;
        private PasswordDigestDocument _original;
        private IPasswordCollection _passwordCollection;

        [ SetUp ]
        public void SetUpModelFactory( )
        {
            _original = new PasswordDigestDocumentBuilder
                            {
                                Key = "abij",
                                Hash = new byte[ ] { 0x12, 0x78, 0x56, 0x34 },
                                Iteration = 3,
                                PasswordGenerator = PasswordGenerators2.Full,
                                CreatedOn = new DateTime( 2011, 11, 1 ),
                                ModifiedOn = new DateTime( 2011, 11, 3 ),
                                MasterPasswordId = Guid.NewGuid( ),
                                Note = "AB IJ"
                            };

            _passwordCollection = new PasswordCollection( new InMemoryPasswordData( ) );
            _modelFactory = new PasswordEditorModelFactory( _passwordCollection );
            _model = _modelFactory.CreateModel( _original );
        }

        [ Test ]
        [ Ignore ]
        public void InitiallyHasValuesFromLoadedPassword( )
        {
            // Set up

            // Exercise

            // Verify

            Assert.That( _model.Key, Is.EqualTo( _original.Key ) );
            Assert.That( _model.MasterPassword.Length, Is.EqualTo( 0 ) );
            Assert.That( _model.MasterPasswordId, Is.Null );
            Assert.That( _model.ExpectedMasterPasswordId, Is.EqualTo( _original.MasterPasswordId ) );
            Assert.That( _model.IsDirty, Is.False );
            Assert.That( _model.CanSave, Is.False );
            Assert.That( _model.CanDelete, Is.True );
            Assert.That( _model.DerivedPasswords,
                         Is.EquivalentTo( PasswordGenerators2.Generators.Keys.Select( g => IsPasswordModel.Empty( g, 3 ) ) )
                             .Using( new DerivedPasswordEqualityComparer( ) ) );
        }
    }
}