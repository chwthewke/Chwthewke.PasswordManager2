namespace Chwthewke.PasswordManager.Migration
{
    public class LegacyItem
    {
        public LegacyItem( string key, bool isAlphanumeric )
        {
            Key = key;
            IsAlphanumeric = isAlphanumeric;
        }

        public string Key { get; private set; }
        public bool IsAlphanumeric { get; private set; }

        public bool Equals( LegacyItem other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other.Key, Key ) && other.IsAlphanumeric.Equals( IsAlphanumeric );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType( ) != typeof ( LegacyItem ) ) return false;
            return Equals( ( LegacyItem ) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                return ( ( Key != null ? Key.GetHashCode( ) : 0 ) * 397 ) ^ IsAlphanumeric.GetHashCode( );
            }
        }

        public static bool operator ==( LegacyItem left, LegacyItem right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( LegacyItem left, LegacyItem right )
        {
            return !Equals( left, right );
        }

        public override string ToString( )
        {
            return string.Format( "Key: {0}, IsAlphanumeric: {1}", Key, IsAlphanumeric );
        }
    }
}