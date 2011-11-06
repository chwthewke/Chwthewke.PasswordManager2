using System;

namespace Chwthewke.PasswordManager.Engine
{
    public class DerivedPassword : IEquatable<DerivedPassword>
    {
        public DerivedPassword( string password, PasswordDigest2 digest )
        {
            if ( password == null )
                throw new ArgumentNullException( "password" );
            if (digest == null)
                throw new ArgumentNullException( "digest" );
            
            Password = password;
            Digest = digest;
        }

        public string Password { get; private set; }
        public PasswordDigest2 Digest { get; private set; }

        public bool Equals( DerivedPassword other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other.Password, Password ) && Equals( other.Digest, Digest );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType( ) != typeof ( DerivedPassword ) ) return false;
            return Equals( (DerivedPassword) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                return ( ( Password != null ? Password.GetHashCode( ) : 0 ) * 397 ) ^ ( Digest != null ? Digest.GetHashCode( ) : 0 );
            }
        }
    }
}