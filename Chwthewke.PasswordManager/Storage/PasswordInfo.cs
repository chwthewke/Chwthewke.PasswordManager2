using System;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordInfo : IEquatable<PasswordInfo>
    {
        public string Key { get; private set; }

        public byte[ ] Hash { get; private set; }

        public Guid MasterPasswordId { get; private set; }

        public Guid PasswordSettingsId { get; private set; }

        public DateTime CreationTime { get; private set; }

        public string Note { get; private set; }

        public PasswordInfo( string key,
                             byte[ ] hash,
                             Guid masterPasswordId,
                             Guid passwordFactoryId,
                             DateTime creationTime,
                             string note )
        {
            if ( key == null )
                throw new ArgumentNullException( "key" );
            if ( hash == null )
                throw new ArgumentNullException( "hash" );

            Key = key;
            Hash = hash;
            MasterPasswordId = masterPasswordId;
            PasswordSettingsId = passwordFactoryId;
            CreationTime = creationTime;
            Note = note;
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType( ) != typeof ( PasswordInfo ) ) return false;
            return Equals( ( PasswordInfo ) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                int result = Key.GetHashCode( );
                result = ( result * 397 ) ^ Hash.GetHashCode( );
                result = ( result * 397 ) ^ MasterPasswordId.GetHashCode( );
                result = ( result * 397 ) ^ CreationTime.GetHashCode( );
                result = ( result * 397 ) ^ ( Note != null ? Note.GetHashCode( ) : 0 );
                return result;
            }
        }

        public bool Equals( PasswordInfo other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other.Key, Key ) && other.Hash.SequenceEqual( Hash ) &&
                   other.MasterPasswordId.Equals( MasterPasswordId ) && other.CreationTime.Equals( CreationTime ) &&
                   Equals( other.Note, Note );
        }

        public static bool operator ==( PasswordInfo left, PasswordInfo right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( PasswordInfo left, PasswordInfo right )
        {
            return !Equals( left, right );
        }
    }
}