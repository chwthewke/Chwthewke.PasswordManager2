using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage.Serialization;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordDictionary : IPasswordStorageService, IExportablePasswordCollection
    {
        internal const string Salt = "mBlw1:e[9-@r";

        private readonly IDictionary<string, PasswordData> _passwords = new Dictionary<string, PasswordData>( );

        internal IPassword this[ string key ] { get { return new PasswordWrapper( this, key ); } }

        internal PasswordData GetPasswordData( string key )
        {
            return _passwords.ContainsKey( key ) ? _passwords[ key ] : null;
        }

        internal void SetPasswordData( string key, PasswordData data )
        {
            _passwords[ key ] = data;
        }

        internal void RemovePasswordData( string key )
        {
            _passwords.Remove( key );
        }

        IEnumerable<PasswordDTO> IExportablePasswordCollection.ExportPasswords( )
        {
            foreach ( string key in _passwords.Keys )
            {
                PasswordData passwordData = _passwords[ key ];
                yield return new PasswordDTO { Key = key, PasswordType = passwordData.Type, Hash = passwordData.Hash };
            }
        }

        void IExportablePasswordCollection.ImportPasswords( IEnumerable<PasswordDTO> passwordDtos )
        {
            _passwords.Clear( );
            foreach ( PasswordDTO dto in passwordDtos )
            {
                SetPasswordData( dto.Key, new PasswordData { Type = dto.PasswordType, Hash = dto.Hash } );
            }
        }

        public IEnumerable<string> KnownKeys { get { return new ReadOnlyCollection<string>( _passwords.Keys.ToList( ) ); } }

        public bool HasPassword( string key )
        {
            return _passwords.ContainsKey( key );
        }

        public bool ValidatePassword( string key, string password, PasswordType type )
        {
            IPassword passwordObject = this[ key ];
            return passwordObject.PasswordType == type && passwordObject.ValidatePassword( password );
        }

        public void SetPassword( string key, string password, PasswordType type )
        {
            IPassword passwordObject = this[ key ];
            passwordObject.SetPassword( password, type );
        }

        public void ClearPassword( string key )
        {
            IPassword passwordObject = this[ key ];
            passwordObject.Clear( );
        }

        private readonly Func<IExportablePasswordCollection, ISerializer> _serializerConfig =
            s => new CsvPasswordSerializer( s );

        public ISerializer Serializer { get { return _serializerConfig( this ); } }
    }
}