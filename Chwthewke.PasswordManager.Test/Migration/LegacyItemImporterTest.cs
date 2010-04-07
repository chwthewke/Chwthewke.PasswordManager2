using System;
using System.Collections.Generic;
using System.IO;
using System.Linq.Expressions;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Migration;
using Chwthewke.PasswordManager.Storage;
using Chwthewke.PasswordManager.Test.Engine;
using Chwthewke.PasswordManager.Test.Storage;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Migration
{
    public class LegacyItemImporterTest
    {
        private Mock<IPasswordStore> _passwordStoreMock;
        private Mock<IPasswordDigester> _passwordDigesterMock;
        private Mock<IPasswordStoreSerializer> _serializerMock;
        private LegacyItemImporter _importer;

        [ SetUp ]
        public void SetUpImporter( )
        {
            _passwordStoreMock = new Mock<IPasswordStore>( );

            _passwordDigesterMock = new Mock<IPasswordDigester>( );

            _serializerMock = new Mock<IPasswordStoreSerializer>( );

            _importer = new LegacyItemImporter( _passwordStoreMock.Object, _passwordDigesterMock.Object,
                                                _serializerMock.Object );
        }

        [ Test ]
        public void ImportDigestsItems( )
        {
            // Setup
            IEnumerable<LegacyItem> legacyItems = new[ ]
                                                      {
                                                          new LegacyItem( "aKey", false ),
                                                          new LegacyItem( "anotherKey", true ),
                                                      };
            SecureString masterPassword = Util.Secure( "p@ssw" );

            // Exercise
            _importer.Import( legacyItems, masterPassword );
            // Verify
            _passwordDigesterMock.Verify( DoDigest( "aKey", masterPassword, PasswordGenerators.Full ) );
            _passwordDigesterMock.Verify( DoDigest( "anotherKey", masterPassword, PasswordGenerators.AlphaNumeric ) );
        }

        [ Test ]
        public void ImportAddsDigestsToStore( )
        {
            // Setup
            SecureString masterPassword = Util.Secure( "p@ssw" );

            PasswordDigest passwordDigest1 = new PasswordDigestBuilder( ).WithKey( "aKey" );
            _passwordDigesterMock.Setup( DoDigest( "aKey", masterPassword, PasswordGenerators.Full ) )
                .Returns( passwordDigest1 );
            PasswordDigest passwordDigest2 = new PasswordDigestBuilder( ).WithKey( "anotherKey" );
            _passwordDigesterMock.Setup( DoDigest( "anotherKey", masterPassword, PasswordGenerators.AlphaNumeric ) )
                .Returns( passwordDigest2 );

            IEnumerable<LegacyItem> legacyItems = new[ ]
                                                      {
                                                          new LegacyItem( "aKey", false ),
                                                          new LegacyItem( "anotherKey", true ),
                                                      };

            // Exercise
            _importer.Import( legacyItems, masterPassword );
            // Verify
            _passwordStoreMock.Verify( s => s.AddOrUpdate( passwordDigest1 ) );
            _passwordStoreMock.Verify( s => s.AddOrUpdate( passwordDigest2 ) );
        }

        [ Test ]
        public void SaveCallsSerializer( )
        {
            // Setup

            try
            {
                // Exercise
                _importer.Save( "__tmp__" );
                // Verify
                _serializerMock.Verify( s => s.Save( _passwordStoreMock.Object,
                                                     It.Is<FileStream>( fs => fs.Name.EndsWith( @"\__tmp__" ) ) ) );
            }
            finally
            {
                // Teardown
                File.Delete( "__tmp__" );
            }
        }


        private static Expression<Func<IPasswordDigester, PasswordDigest>> DoDigest( string akey,
                                                                                     SecureString masterPassword,
                                                                                     IPasswordGenerator
                                                                                         passwordGenerator )
        {
            return d => d.Digest( akey,
                                  passwordGenerator.MakePassword( akey, masterPassword ),
                                  It.IsAny<Guid>( ),
                                  passwordGenerator.Id,
                                  It.IsAny<string>( ) );
        }
    }
}