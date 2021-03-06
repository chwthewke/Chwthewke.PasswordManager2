﻿using System;
using System.Collections.Generic;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class PasswordRepositoryTest
    {
        private InMemoryPasswordData _inMemoryPasswordData;
        private IPasswordRepository _passwordRepository;
        private List<PasswordDigestDocument> _allPasswords;

        [ SetUp ]
        public void SetupPasswordCollection( )
        {
            _inMemoryPasswordData = new InMemoryPasswordData( );
            _allPasswords = new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl }.ToList( );
            _inMemoryPasswordData.SavePasswords( _allPasswords );

            _passwordRepository = new PasswordRepository( _inMemoryPasswordData );
        }

        [ Test ]
        public void LoadPasswordsReturnsAllNonDeletedPasswordsInData( )
        {
            // Set up
            // Exercise
            var passwords = _passwordRepository.LoadPasswords( );
            // Verify
            Assert.That( passwords, Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void LoadPasswordReturnsMatchingPassword( )
        {
            // Set up

            // Exercise
            var password = _passwordRepository.LoadPassword( "abcd" );
            // Verify
            Assert.That( password, Is.EqualTo( TestPasswords.Abcd ) );
        }

        [ Test ]
        public void LoadMissingPasswordReturnsNull( )
        {
            // Set up

            // Exercise
            var password = _passwordRepository.LoadPassword( "abce" );
            // Verify
            Assert.That( password, Is.Null );
        }

        [ Test ]
        public void LoadDeletedPasswordReturnsNull( )
        {
            // Set up

            // Exercise
            var password = _passwordRepository.LoadPassword( "ijkl" );
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
            var saved = _passwordRepository.SavePassword( password );
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
            _passwordRepository.SavePassword( passwordV1 );

            PasswordDigestDocument passwordV2 = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "azer",
                                                        Hash = new byte[ ] { 0x56, 0x78 },
                                                        ModifiedOn = new DateTime( 2011, 11, 2 )
                                                    };

            // Exercise
            var saved = _passwordRepository.SavePassword( passwordV2 );

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
            _passwordRepository.SavePassword( passwordV1 );

            PasswordDigestDocument passwordV2 = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "azer",
                                                        Hash = new byte[ ] { 0x56, 0x78 },
                                                        ModifiedOn = new DateTime( 2011, 11, 2 )
                                                    };

            // Exercise
            var saved = _passwordRepository.SavePassword( passwordV2 );

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
            var saved = _passwordRepository.SavePassword( password );

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
            var saved = _passwordRepository.SavePassword( password );

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
            _passwordRepository.SavePassword( password.Delete( new DateTime( 2011, 11, 4 ) ) );

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
            var updated = _passwordRepository.UpdatePassword( password, updatedPassword );
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
            var updated = _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPassword );
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
            _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPasswordV1 );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
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
            _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPasswordV1 );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
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
            _passwordRepository.UpdatePassword( TestPasswords.Abcd, deletedPassword );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
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
            _passwordRepository.UpdatePassword( TestPasswords.Abcd, deletedPassword );
            PasswordDigestDocument updatedPasswordV2 = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 6 ) );

            // Exercise
            var updated = _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPasswordV2 );
            // Verify
            Assert.That( updated, Is.False );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { deletedPassword, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void UpdateUnchangedExistingPasswordWithDeletedDeletesIt( )
        {
            // Set up
            PasswordDigestDocument updatedPassword = TestPasswords.Abcd.Delete( new DateTime( 2011, 11, 5 ) );
            // Exercise
            var updated = _passwordRepository.UpdatePassword( TestPasswords.Abcd, updatedPassword );
            // Verify
            Assert.That( updated, Is.True );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { updatedPassword, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
            Assert.That( _passwordRepository.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Efgh } ) );
        }

        [ Test ]
        public void DeletePassswordIsLikeUpdatingItWithDeletedPassword( )
        {
            // Set up
            var passwordToDelete = _passwordRepository.LoadPassword( "abcd" );
            // Exercise
            var deleted = _passwordRepository.DeletePassword( passwordToDelete, new DateTime( 2011, 11, 11 ) );
            // Verify
            Assert.That( deleted, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abcd" ), Is.Null );
        }

        [ Test ]
        public void SaveOverPreviouslyDeletedPasswordOverwritesIt( )
        {
            // Set up
            var passwordToDelete = _passwordRepository.LoadPassword( "abcd" );
            _passwordRepository.DeletePassword( passwordToDelete, new DateTime( 2011, 11, 11 ) );

            PasswordDigestDocument newVersion = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "abcd",
                                                        Hash = new byte[ ] { 0x65 },
                                                        PasswordGenerator = PasswordGenerators.LegacyAlphaNumeric,
                                                        Iteration = 2,
                                                        MasterPasswordId = Guid.NewGuid( ),
                                                        CreatedOn = new DateTime( 2011, 11, 12 ),
                                                        ModifiedOn = new DateTime( 2011, 11, 13 ),
                                                        Note = ""
                                                    };

            // Exercise
            var saved = _passwordRepository.SavePassword( newVersion );
            // Verify
            Assert.That( saved, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abcd" ), Is.EqualTo( newVersion ) );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ).Where( p => p.Key == "abcd" ).Count( ), Is.EqualTo( 1 ) );
        }

        [ Test ]
        public void UpdatePreviouslyDeletedPasswordOverwritesIt( )
        {
            // Set up
            var passwordToDelete = _passwordRepository.LoadPassword( "abcd" );
            _passwordRepository.DeletePassword( passwordToDelete, new DateTime( 2011, 11, 11 ) );

            PasswordDigestDocument newVersion = new PasswordDigestDocumentBuilder
                                                    {
                                                        Key = "abcd",
                                                        Hash = new byte[ ] { 0x65 },
                                                        PasswordGenerator = PasswordGenerators.LegacyAlphaNumeric,
                                                        Iteration = 2,
                                                        MasterPasswordId = Guid.NewGuid( ),
                                                        CreatedOn = new DateTime( 2011, 11, 12 ),
                                                        ModifiedOn = new DateTime( 2011, 11, 13 ),
                                                        Note = ""
                                                    };

            PasswordDigestDocument deleted = _inMemoryPasswordData.LoadPasswords( ).First( p => p.Key == "abcd" );
            // Exercise
            var updated = _passwordRepository.UpdatePassword( deleted, newVersion );
            // Verify
            Assert.That( updated, Is.True );
            Assert.That( _passwordRepository.LoadPassword( "abcd" ), Is.EqualTo( newVersion ) );
            Assert.That( _inMemoryPasswordData.LoadPasswords( ).Where( p => p.Key == "abcd" ).Count( ), Is.EqualTo( 1 ) );
        }

        [ Test ]
        public void MergeAddsOurPasswordsToTarget( )
        {
            // Set up
            var sourcePasswords = new List<PasswordDigestDocument> { TestPasswords.Abcd };

            var targetData = new InMemoryPasswordData( );
            targetData.SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Efgh, TestPasswords.Ijkl } );
            IPasswordRepository targetRepository = new PasswordRepository( targetData );

            // Exercise
            targetRepository.Merge( sourcePasswords );
            // Verify
            Assert.That( targetData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void MergeAddsOurDeletedPasswordsToTarget( )
        {
            // Set up
            var sourcePasswords = new List<PasswordDigestDocument> { TestPasswords.Abcd, TestPasswords.Ijkl };

            var targetData = new InMemoryPasswordData( );
            targetData.SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Efgh } );
            IPasswordRepository targetRepository = new PasswordRepository( targetData );

            // Exercise
            targetRepository.Merge( sourcePasswords );
            // Verify
            Assert.That( targetData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { TestPasswords.Abcd, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void MergeAddsOurMoreRecentPasswordsToTarget( )
        {
            // Set up
            PasswordDigestDocument abcdUpdated = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 9 ) );

            var sourcePasswords = new List<PasswordDigestDocument> { abcdUpdated, TestPasswords.Ijkl };

            var targetData = new InMemoryPasswordData( );
            targetData.SavePasswords( new List<PasswordDigestDocument> { TestPasswords.Abcd, TestPasswords.Efgh } );
            IPasswordRepository targetRepository = new PasswordRepository( targetData );

            // Exercise
            targetRepository.Merge( sourcePasswords );
            // Verify
            Assert.That( targetData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { abcdUpdated, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void MergeSkipsOurLessRecentPasswordsToTarget( )
        {
            // Set up
            PasswordDigestDocument abcdUpdated = Update( TestPasswords.Abcd, new DateTime( 2011, 11, 9 ) );

            var sourcePasswords = new List<PasswordDigestDocument> { TestPasswords.Abcd, TestPasswords.Ijkl };

            var targetData = new InMemoryPasswordData( );
            targetData.SavePasswords( new List<PasswordDigestDocument> { abcdUpdated, TestPasswords.Efgh } );
            IPasswordRepository targetRepository = new PasswordRepository( targetData );

            // Exercise
            targetRepository.Merge( sourcePasswords );
            // Verify
            Assert.That( targetData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { abcdUpdated, TestPasswords.Efgh, TestPasswords.Ijkl } ) );
        }

        [ Test ]
        public void MergeRecognizesIdenticalMasterPasswords( )
        {
            // Set up
            Guid sourceMasterPasswordId = Guid.NewGuid( );
            var sourceAbcd = WithMasterPasswordId( TestPasswords.Abcd, sourceMasterPasswordId );
            var sourceEfgh = WithMasterPasswordId( TestPasswords.Efgh, sourceMasterPasswordId );

            var sourcePasswords = new List<PasswordDigestDocument> { sourceAbcd, sourceEfgh };

            Guid targetMasterPasswordId = Guid.NewGuid( );
            var targetAbcd = WithMasterPasswordId( TestPasswords.Abcd, targetMasterPasswordId );
            var targetIjkl = WithMasterPasswordId( TestPasswords.Ijkl, targetMasterPasswordId );

            var targetData = new InMemoryPasswordData( );
            targetData.SavePasswords( new List<PasswordDigestDocument> { targetAbcd, targetIjkl } );
            IPasswordRepository targetRepository = new PasswordRepository( targetData );

            // Exercise
            targetRepository.Merge( sourcePasswords );

            // Verify
            Assert.That( targetData.LoadPasswords( ),
                         Is.EquivalentTo( new[ ] { targetAbcd, targetIjkl, WithMasterPasswordId( sourceEfgh, targetMasterPasswordId ) } ) );
        }

        private PasswordDigestDocument Update( PasswordDigestDocument source, DateTime updatedOn )
        {
            return new PasswordDigestDocument( source.Digest, source.MasterPasswordId, source.CreatedOn, updatedOn, source.Note );
        }

        private PasswordDigestDocument WithMasterPasswordId( PasswordDigestDocument source, Guid masterPasswordId )
        {
            return new PasswordDigestDocument( source.Digest, masterPasswordId, source.CreatedOn, source.ModifiedOn, source.Note );
        }
    }
}