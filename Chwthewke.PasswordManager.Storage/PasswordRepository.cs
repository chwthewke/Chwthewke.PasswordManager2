using System;
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

                _passwordData = value;
            }
        }


        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            TryLoad( );
            return new List<PasswordDigestDocument>( _passwordsCache.Values.Where( p => !p.IsDeleted ) );
        }


        public PasswordDigestDocument LoadPassword( string key )
        {
            TryLoad( );

            if ( !_passwordsCache.ContainsKey( key ) )
                return null;

            PasswordDigestDocument password = _passwordsCache[ key ];
            return password.IsDeleted ? null : password;
        }

        public bool SavePassword( PasswordDigestDocument password )
        {
            if ( password == null )
                throw new ArgumentNullException( "password" );

            if ( password.IsDeleted )
                throw new ArgumentException( "A new password must have non-empty hash.", "password" );

            // TODO internal IDisposable ?
            Load( );

            var mergedPasswords = Merge( new[ ] { password }, false );

            Save( );
            return mergedPasswords.Count( ) > 0;
        }

        public bool UpdatePassword( PasswordDigestDocument original, PasswordDigestDocument password )
        {
            if ( original == null )
                throw new ArgumentNullException( "original" );
            if ( password == null )
                throw new ArgumentNullException( "password" );

            if ( original.Key != password.Key )
                throw new ArgumentException( "Replacement must have same key as original.", "password" );

            Load( );

            if ( !_passwordsCache.ContainsKey( original.Key ) )
                return false;
            var mergedPasswords = Merge( new[ ] { password }, false );

            Save( );
            return mergedPasswords.Count( ) > 0;
        }

        public bool DeletePassword( PasswordDigestDocument password, DateTime deletedOn )
        {
            if ( password == null )
                throw new ArgumentNullException( "password" );

            return UpdatePassword( password, password.Delete( deletedOn ) );
        }

        public void Merge( IEnumerable<PasswordDigestDocument> passwordData )
        {
            Load( );
            Merge( passwordData, true );
            Save( );
        }

        private void TryLoad( )
        {
            try
            {
                Load( );
            }
            catch ( PasswordsFileException )
            {
            }
        }

        private IEnumerable<PasswordDigestDocument> Merge( IEnumerable<PasswordDigestDocument> newPasswords, bool mergeMasterPasswordIds )
        {
            var newPasswordsList = new List<PasswordDigestDocument>( newPasswords );
            IMasterPasswordIdMapper mapper;
            if ( mergeMasterPasswordIds )
                mapper = new MergeMasterPasswordMapper( newPasswordsList, _passwordsCache.Values );
            else
                mapper = new IdentityMasterPasswordMapper( );

            return Merge( newPasswordsList, mapper );
        }

        private IEnumerable<PasswordDigestDocument> Merge( IEnumerable<PasswordDigestDocument> newPasswords, IMasterPasswordIdMapper mapper )
        {
            IList<PasswordDigestDocument> result = new List<PasswordDigestDocument>( );
            foreach ( var newPassword in newPasswords )
            {
                DateTime creation;
                if ( _passwordsCache.ContainsKey( newPassword.Key ) )
                {
                    var oldPassword = _passwordsCache[ newPassword.Key ];
                    if ( oldPassword.ModifiedOn.CompareTo( newPassword.ModifiedOn ) > 0 )
                        continue;
                    if ( oldPassword.IsDeleted )
                        creation = newPassword.CreatedOn;
                    else
                        creation = new[ ] { oldPassword.CreatedOn, newPassword.CreatedOn }.Min( );
                }
                else
                {
                    creation = newPassword.CreatedOn;
                }

                var mergedPassword = new PasswordDigestDocument( newPassword.Digest,
                                                                 mapper.MapMasterPassword( newPassword.MasterPasswordId ),
                                                                 creation,
                                                                 newPassword.ModifiedOn,
                                                                 newPassword.Note );
                _passwordsCache[ mergedPassword.Key ] = mergedPassword;
                result.Add( mergedPassword );
            }
            return result;
        }


        private void Load( )
        {
            _passwordsCache = _passwordData.LoadPasswords( ).ToDictionary( p => p.Key );
        }

        private void Save( )
        {
            _passwordData.SavePasswords( _passwordsCache.Values.ToList( ) );
        }

        private IDictionary<String, PasswordDigestDocument> _passwordsCache = new Dictionary<string, PasswordDigestDocument>( );

        private interface IMasterPasswordIdMapper
        {
            Guid MapMasterPassword( Guid id );
        }

        private class MergeMasterPasswordMapper : IMasterPasswordIdMapper
        {
            public MergeMasterPasswordMapper( IEnumerable<PasswordDigestDocument> sourcePasswords,
                                              IEnumerable<PasswordDigestDocument> targetPasswords )
            {
                var masterPasswordIdMerges = sourcePasswords
                    .SelectMany( s => targetPasswords
                                          .Where( ShouldMergeWith( s ) )
                                          .Select( t => new { Source = s.MasterPasswordId, Target = t.MasterPasswordId } ) )
                    .Distinct( );

                foreach ( var merge in masterPasswordIdMerges )
                {
                    if ( !_mapping.ContainsKey( merge.Source ) )
                        _mapping[ merge.Source ] = merge.Target;
                }
            }

            public Guid MapMasterPassword( Guid id )
            {
                return _mapping.ContainsKey( id ) ? _mapping[ id ] : id;
            }

            private Func<PasswordDigestDocument, bool> ShouldMergeWith( PasswordDigestDocument document )
            {
                return d => d.Key == document.Key &&
                            d.Hash.SequenceEqual( document.Hash ) &&
                            d.MasterPasswordId != document.MasterPasswordId;
            }

            private readonly IDictionary<Guid, Guid> _mapping = new Dictionary<Guid, Guid>( );
        }

        private class IdentityMasterPasswordMapper : IMasterPasswordIdMapper
        {
            public Guid MapMasterPassword( Guid id )
            {
                return id;
            }
        }


        private IPasswordData _passwordData;
    }
}