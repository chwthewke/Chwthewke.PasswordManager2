﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordRepository : IPasswordRepository
    {
        public PasswordRepository( IPasswordData passwordData )
        {
            _passwordData = passwordData;
        }

        public PasswordDigestDocument LoadPassword( string key )
        {
            if ( key == null )
                throw new ArgumentNullException( "key" );

            PasswordDigestDocument password = LoadPasswordInternal( key );
            if ( password == null || password.IsDeleted )
                return null;
            return password;
        }

        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            return LoadPasswordsInternal( ).Where( p => !p.IsDeleted ).ToList( );
        }

        public bool UpdatePassword( PasswordDigestDocument original, PasswordDigestDocument password )
        {
            if ( original == null )
                throw new ArgumentNullException( "original" );
            if ( password == null )
                throw new ArgumentNullException( "password" );

            if ( original.Key != password.Key )
                throw new ArgumentException( "Replacement must have same key as original.", "password" );

            var replacedPassword = LoadPasswordInternal( original.Key );
            if ( replacedPassword == null )
                return false;

            if ( !replacedPassword.Equals( original ) &&
                 replacedPassword.ModifiedOn.CompareTo( password.ModifiedOn ) > 0 )
                return false;

            return ReplacePassword( password, replacedPassword );
        }

        public bool SavePassword( PasswordDigestDocument password )
        {
            if ( password == null )
                throw new ArgumentNullException( "password" );
            if ( password.IsDeleted )
                throw new ArgumentException( "A new password must have non-empty hash.", "password" );

            return SaveInternal( password );
        }


        public bool DeletePassword( PasswordDigestDocument password, DateTime deletedOn )
        {
            if ( password == null )
                throw new ArgumentNullException( "password" );

            return UpdatePassword( password, password.Delete( deletedOn ) );
        }

        public void MergeInto( IPasswordRepository target )
        {
            if ( !( target is PasswordRepository ) )
                return;
            PasswordRepository targetRepository = target as PasswordRepository;

            var updateMasterPasswordId = MasterPasswordIdMergeFunction( targetRepository );

            foreach ( PasswordDigestDocument password in LoadPasswordsInternal( ) )
            {
                var copy = new PasswordDigestDocument( password.Digest, updateMasterPasswordId( password.MasterPasswordId ),
                                                       password.CreatedOn, password.ModifiedOn, password.Note );

                targetRepository.SaveInternal( copy );
            }
        }

        private Func<Guid, Guid> MasterPasswordIdMergeFunction( PasswordRepository targetRepository )
        {
            var masterPasswordIdMerges = LoadPasswordsInternal( )
                .SelectMany( s => targetRepository.LoadPasswordsInternal( )
                                      .Where( ShouldMergeWith( s ) )
                                      .Select( t => new { Source = s.MasterPasswordId, Target = t.MasterPasswordId } ) )
                .Distinct( );

            return g => masterPasswordIdMerges
                            .Where( m => m.Source == g )
                            .Select( m => m.Target )
                            .DefaultIfEmpty( g )
                            .First( );
        }

        private Func<PasswordDigestDocument, bool> ShouldMergeWith( PasswordDigestDocument document )
        {
            return d => d.Key == document.Key &&
                        d.Hash.SequenceEqual( document.Hash ) &&
                        d.MasterPasswordId != document.MasterPasswordId;
        }

        private IList<PasswordDigestDocument> LoadPasswordsInternal( )
        {
            return _passwordData.LoadPasswords( );
        }

        private PasswordDigestDocument LoadPasswordInternal( string key )
        {
            return FindPassword( key, LoadPasswordsInternal( ) );
        }

        private PasswordDigestDocument FindPassword( string key, IEnumerable<PasswordDigestDocument> passwords )
        {
            return passwords.FirstOrDefault( d => d.Digest.Key == key );
        }

        private bool SaveInternal( PasswordDigestDocument password )
        {
            var replacedPassword = LoadPasswordInternal( password.Key );

            if ( replacedPassword != null &&
                 replacedPassword.ModifiedOn.CompareTo( password.ModifiedOn ) > 0 )
                return false;

            return ReplacePassword( password, replacedPassword );
        }

        private bool ReplacePassword( PasswordDigestDocument replacementPassword, PasswordDigestDocument replacedPassword )
        {
            IList<PasswordDigestDocument> passwords = LoadPasswordsInternal( );
            passwords.Remove( replacedPassword );
            passwords.Add( replacementPassword );
            _passwordData.SavePasswords( passwords );
            return true;
        }

        private readonly IPasswordData _passwordData;
    }
}