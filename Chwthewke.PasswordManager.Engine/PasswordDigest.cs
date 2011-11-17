using System;
using System.Linq;

namespace Chwthewke.PasswordManager.Engine
{
    public class PasswordDigest : IEquatable<PasswordDigest>
    {
        public PasswordDigest( string key, byte[ ] hash, int iteration, Guid passwordGenerator )
        {
            if ( key == null )
                throw new ArgumentNullException( "key" );
            if ( hash == null )
                throw new ArgumentNullException( "hash" );

            Key = key;
            Hash = hash;
            Iteration = iteration;
            PasswordGenerator = passwordGenerator;
        }

        public string Key { get; private set; }
        public byte[ ] Hash { get; private set; }
        public int Iteration { get; private set; }
        public Guid PasswordGenerator { get; private set; }

        public bool Equals( PasswordDigest other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other.Key, Key ) && other.Hash.SequenceEqual( Hash ) && other.Iteration == Iteration &&
                   other.PasswordGenerator.Equals( PasswordGenerator );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType( ) != typeof ( PasswordDigest ) ) return false;
            return Equals( (PasswordDigest) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                int result = Key.GetHashCode( );
                result = ( result * 397 ) ^ Iteration;
                result = ( result * 397 ) ^ PasswordGenerator.GetHashCode( );
                return result;
            }
        }

        public override string ToString( )
        {
            return string.Format( "Key: {0}, Hash: {1}, Iteration: {2}, PasswordGenerator: {3}",
                                  Key, string.Join( ", ", Hash ), Iteration, PasswordGenerator );
        }
    }
}