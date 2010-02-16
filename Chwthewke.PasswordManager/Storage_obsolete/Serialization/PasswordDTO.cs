using System.Linq;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage.Serialization
{
    public class PasswordDTO
    {
        internal string Key { get; set; }
        internal PasswordType PasswordType { get; set; }
        internal byte[ ] Hash { get; set; }

        public bool Equals( PasswordDTO other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;

            if ( Hash == null )
                return other.Hash == null;

            return Equals( other.Key, Key ) && Equals( other.PasswordType, PasswordType ) &&
                   Hash.SequenceEqual( other.Hash );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType( ) != typeof ( PasswordDTO ) ) return false;
            return Equals( ( PasswordDTO ) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                int result = ( Key != null ? Key.GetHashCode( ) : 0 );
                result = ( result * 397 ) ^ ( PasswordType != null ? PasswordType.GetHashCode( ) : 0 );
                result = ( result * 397 ) ^ ( Hash != null ? Hash.GetHashCode( ) : 0 );
                return result;
            }
        }
    }
}