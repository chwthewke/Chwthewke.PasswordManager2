using System;
using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordCollectionTest
    {
        private InMemoryPasswordData _inMemoryPasswordData;
        private IPasswordCollection _passwordCollection;
        private List<PasswordDigestDocument> _allPasswords;

        [ SetUp ]
        public void SetupPasswordCollection( )
        {
            _inMemoryPasswordData = new InMemoryPasswordData( );
            _allPasswords = new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl }.ToList( );
            _inMemoryPasswordData.SavePasswords( _allPasswords );

            _passwordCollection = new PasswordCollection( _inMemoryPasswordData );
        }

        [ Test ]
        public void LoadPasswordsReturnsAllNonDeletedPasswordsInData( )
        {
            // Set up
            // Exercise
            var passwords = _passwordCollection.LoadPasswords( );
            // Verify
            Assert.That( passwords, Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void LoadPasswordReturnsMatchingPassword( )
        {
            // Set up

            // Exercise
            var password = _passwordCollection.LoadPassword( "abcd" );
            // Verify
            Assert.That( password, Is.EqualTo( TestPasswords.Abcd ) );
        }

        [ Test ]
        public void LoadMissingPasswordReturnsNull( )
        {
            // Set up

            // Exercise
            var password = _passwordCollection.LoadPassword( "abce" );
            // Verify
            Assert.That( password, Is.Null );
        }

        [ Test ]
        public void LoadDeletedPasswordReturnsNull( )
        {
            // Set up

            // Exercise
            var password = _passwordCollection.LoadPassword( "ijkl" );
            // Verify
            Assert.That( password, Is.Null );
        }

        [ Test ]
        public void SaveNewPasswordAddsItToData( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "azer",
                                                      Hash = new byte[ ] { 0x12, 0x34 }
                                                  };
            // Exercise
            var saved = _passwordCollection.SavePassword( password );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl, password } ) );
        }

        [ Test ]
        public void SaveOverEarlierExistingPasswordOverwritesIt( )
        {
            // Set up
            PasswordDigestDocument passwordV1 = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "azer",
                                                        Hash = new byte[ ] { 0x12, 0x34 },
                                                        ModifiedOn = new DateTime( 2011, 11, 1 )
                                                    };
            _passwordCollection.SavePassword( passwordV1 );

            PasswordDigestDocument passwordV2 = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "azer",
                                                        Hash = new byte[ ] { 0x56, 0x78 },
                                                        ModifiedOn = new DateTime( 2011, 11, 2 )
                                                    };

            // Exercise
            var saved = _passwordCollection.SavePassword( passwordV2 );

            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl, passwordV2 } ) );
        }

        [ Test ]
        public void SaveOverMoreRecentPasswordFails( )
        {
            // Set up
            PasswordDigestDocument passwordV1 = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "azer",
                                                        Hash = new byte[ ] { 0x12, 0x34 },
                                                        ModifiedOn = new DateTime( 2011, 11, 3 )
                                                    };
            _passwordCollection.SavePassword( passwordV1 );

            PasswordDigestDocument passwordV2 = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "azer",
                                                        Hash = new byte[ ] { 0x56, 0x78 },
                                                        ModifiedOn = new DateTime( 2011, 11, 2 )
                                                    };

            // Exercise
            var saved = _passwordCollection.SavePassword( passwordV2 );

            // Verify
            Assert.That( saved, Is.False );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl, passwordV1 } ) );
        }

        [ Test ]
        public void SaveOverEarlierDeletedPasswordOverwritesIt( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ijkl",
                                                      Hash = new byte[ ] { 0x12, 0x34 },
                                                      ModifiedOn = new DateTime( 2011, 11, 6 )
                                                  };

            // Exercise
            var saved = _passwordCollection.SavePassword( password );

            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, password } ) );
        }

        [ Test ]
        public void SaveOverMoreRecentDeletedPasswordFails( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "ijkl",
                                                      Hash = new byte[ ] { 0x12, 0x34 },
                                                      ModifiedOn = new DateTime( 2011, 11, 3 )
                                                  };

            // Exercise
            var saved = _passwordCollection.SavePassword( password );

            // Verify
            Assert.That( saved, Is.False );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), Is.EquivalentTo( _allPasswords ) );
        }

        [ Test ]
        [ ExpectedException( typeof ( ArgumentException ) ) ]
        public void SaveDeletedPasswordFailsWithException( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "erty",
                                                      Hash = new byte[ ] { 0x12, 0x34 },
                                                      ModifiedOn = new DateTime( 2011, 11, 3 )
                                                  };

            // Exercise
            _passwordCollection.SavePassword( password.Delete( new DateTime( 2011, 11, 4 ) ) );

            // Verify
        }

        [ Test ]
        public void UpdateMissingPasswordFails( )
        {
            // Set up
            PasswordDigestDocument password = new PasswordDigestDocumentBuilder
                                                  {
                                                      Key = "azer",
                                                      Hash = new byte[ ] { 0x12, 0x34 },
                                                      ModifiedOn = new DateTime( 2011, 11, 3 )
                                                  };
            PasswordDigestDocument updatedPassword = Update( password, new DateTime( 2011, 11, 5 ) );
            // Exercise
            var updated = _passwordCollection.UpdatePassword( password, updatedPassword );
            // Verify
            Assert.That( updated, Is.False );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), Is.EquivalentTo( _allPasswords ) );
        }

        [ Test ]
        public void UpdateUnchangedExistingPasswordOverwritesIt( )
        {
            // Set up
            PasswordDigestDocument updatedPassword = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 5 ) );
            // Exercise
            var updated = _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPassword );
            // Verify
            Assert.That( updated, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), 
                Is.EquivalentTo( new[ ] { updatedPassword, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void UpdatePasswordModifiedEarlierOverwritesIt( )
        {
            // Set up
            PasswordDigestDocument updatedPasswordV1 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 5 ) );
            _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPasswordV1 );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
            // Verify
            Assert.That( updated, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), 
                Is.EquivalentTo( new[ ] { updatedPasswordV2, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void UpdatePasswordModifiedLaterFails( )
        {
            // Set up
            PasswordDigestDocument updatedPasswordV1 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 7 ) );
            _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPasswordV1 );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
            // Verify
            Assert.That( updated, Is.False );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), 
                Is.EquivalentTo( new[ ] { updatedPasswordV1, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void UpdatePasswordDeletedEarlierOverwritesIt( )
        {
            // Set up
            PasswordDigestDocument deletedPassword = TestPasswords.Abcd.Delete( new DateTime( 2011, 11, 5 ) );
            _passwordCollection.UpdatePassword( TestPasswords.Abcd, deletedPassword );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
            // Verify
            Assert.That( updated, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), 
                Is.EquivalentTo( new[ ] { updatedPasswordV2, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void UpdatePasswordDeletedLaterFails( )
        {
            // Set up
            PasswordDigestDocument deletedPassword = TestPasswords.Abcd.Delete( new DateTime( 2011, 11, 7 ) );
            _passwordCollection.UpdatePassword( TestPasswords.Abcd, deletedPassword );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
            // Verify
            Assert.That( updated, Is.False );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ), 
                Is.EquivalentTo( new[ ] { deletedPassword, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [Test]
        public void UpdateUnchangedExistingPasswordWithDeletedDeletesIt( )
        {
            // Set up
            PasswordDigestDocument updatedPassword = TestPasswords.Abcd.Delete( new DateTime( 2011, 11, 5 ) );
            // Exercise
            var updated = _passwordCollection.UpdatePassword( TestPasswords.Abcd, updatedPassword );
            // Verify
            Assert.That( updated, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                Is.EquivalentTo( new[ ] { updatedPassword, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
            Assert.That( _passwordCollection.LoadPasswords( ),
                Is.EquivalentTo( new[ ] { TestPasswords.Efgh } ) );
        }



        private PasswordDigestDocument Update( PasswordDigestDocument source, DateTime updatedOn )
        {
            PasswordDigest2 newDigest = source.Digest;
            if ( newDigest.Key != source.Key )
                throw new ArgumentException( "Invalid key in new Digest.", "newDigest" );
            return new PasswordDigestDocument( newDigest, source.MasterPasswordId, source.CreatedOn, updatedOn, source.Note );
        }
    }
}