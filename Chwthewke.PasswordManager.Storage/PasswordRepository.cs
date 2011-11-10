using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordRepository : IPasswordRepository
    {
        public PasswordRepository( IPasswordData passwordData )
        {
            if ( passwordData == null )
                throw new ArgumentNullException( "passwordData" );
            _passwordData = passwordData;
        }

        public IPasswordData PasswordData
        {
            get { return _passwordData; }
            set
            {
                if ( value == null )
                    throw new ArgumentNullException( "value" );

                var oldPasswordData = _passwordData;
                _passwordData = value;
                Merge( new PasswordRepository( oldPasswordData ) );
            }
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

            ReplacePassword( password, replacedPassword );
            return true;
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

        public void Merge( PasswordRepository passwordRepository )
        {
            IList<PasswordDigestDocument> passwords = passwordRepository.LoadPasswordsInternal( );

            var updateMasterPasswordId = MasterPasswordIdMergeFunction( passwords );

            SaveInternal( passwords.Select( p => new PasswordDigestDocument( p.Digest, updateMasterPasswordId( p.MasterPasswordId ),
                                                                             p.CreatedOn, p.ModifiedOn, p.Note ) ) );
        }

        public void Merge( IPasswordRepository passwordRepository )
        {
            if ( !( passwordRepository is PasswordRepository ) )
                return;
            Merge( passwordRepository as PasswordRepository );
        }

        private Func<Guid, Guid> MasterPasswordIdMergeFunction( IList<PasswordDigestDocument> sourcePasswords )
        {
            var masterPasswordIdMerges = sourcePasswords
                .SelectMany( s => _passwordData.LoadPasswords( )
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

        private bool SaveInternal( IEnumerable<PasswordDigestDocument> passwords )
        {
            IList<PasswordDigestDocument[ ]> replacements = passwords
                .SelectMany( PasswordReplacement )
                .ToList( );
            var replacedPasswords = replacements
                .Select( r => r[ 0 ] )
                .Where( p => p != null );
            var replacementPasswords = replacements
                .Select( r => r[ 1 ] )
                .ToList( );

            if ( replacementPasswords.Count( ) == 0 )
                return false;

            ReplacePasswords( replacementPasswords, replacedPasswords );
            return true;
        }

        private bool SaveInternal( PasswordDigestDocument password )
        {
            return SaveInternal( new List<PasswordDigestDocument> { password } );
        }

        private IEnumerable<PasswordDigestDocument[ ]> PasswordReplacement( PasswordDigestDocument password )
        {
            var replacedPassword = LoadPasswordInternal( password.Key );
            if ( replacedPassword == null )
                yield return new[ ] { null, password };
            else if ( replacedPassword.ModifiedOn.CompareTo( password.ModifiedOn ) <= 0 )
                yield return new[ ] { replacedPassword, password };
            yield break;
        }

        private void ReplacePasswords( IEnumerable<PasswordDigestDocument> replacementPasswords,
                                       IEnumerable<PasswordDigestDocument> replacedPasswords )
        {
            IList<PasswordDigestDocument> passwords =
                LoadPasswordsInternal( )
                    .Except( replacedPasswords )
                    .Concat( replacementPasswords )
                    .OrderBy( p => p.Key )
                    .ToList( );
            _passwordData.SavePasswords( passwords );
        }

        private void ReplacePassword( PasswordDigestDocument replacementPassword, PasswordDigestDocument replacedPassword )
        {
            ReplacePasswords( new List<PasswordDigestDocument> { replacementPassword },
                              new List<PasswordDigestDocument> { replacedPassword } );
        }

        private IPasswordData _passwordData;
    }
}