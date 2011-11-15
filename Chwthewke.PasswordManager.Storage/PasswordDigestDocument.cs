using System;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordDigestDocument : IEquatable<PasswordDigestDocument>
    {
        public PasswordDigestDocument( PasswordDigest2 digest, Guid masterPasswordId, DateTime createdOn, DateTime modifiedOn, string note )
        {
            if ( digest == null )
                throw new ArgumentNullException( "digest" );
            if ( note == null )
                throw new ArgumentNullException( "note" );

            _digest = digest;
            _masterPasswordId = masterPasswordId;
            _createdOn = createdOn;
            _modifiedOn = modifiedOn;
            _note = note;
        }

        public PasswordDigest2 Digest
        {
            get { return _digest; }
        }

        public string Key
        {
            get { return _digest.Key; }
        }

        public byte[ ] Hash
        {
            get { return _digest.Hash; }
        }

        public int Iteration
        {
            get { return _digest.Iteration; }
        }

        public Guid PasswordGenerator
        {
            get { return _digest.PasswordGenerator; }
        }

        public Guid MasterPasswordId
        {
            get { return _masterPasswordId; }
        }

        public DateTime CreatedOn
        {
            get { return _createdOn; }
        }

        public DateTime ModifiedOn
        {
            get { return _modifiedOn; }
        }

        public string Note
        {
            get { return _note; }
        }

        public bool IsDeleted
        {
            get { return Digest.Hash.Length == 0; }
        }

        public PasswordDigestDocument Delete( DateTime deletedOn )
        {
            return new PasswordDigestDocument( new PasswordDigest2( _digest.Key, new byte[ 0 ], 0, default( Guid ) ),
                                               default( Guid ), _createdOn, deletedOn, string.Empty );
        }

        public override string ToString( )
        {
            return string.Format( "MasterPasswordId: {0}, CreatedOn: {1}, ModifiedOn: {2}, Note: {3}, Digest: {4}",
                                  _masterPasswordId, _createdOn, _modifiedOn, _note, _digest );
        }

        public bool Equals( PasswordDigestDocument other )
        {
            if ( ReferenceEquals( null, other ) ) return false;
            if ( ReferenceEquals( this, other ) ) return true;
            return Equals( other._digest, _digest ) && other._masterPasswordId.Equals( _masterPasswordId ) &&
                   other._createdOn.Equals( _createdOn ) && other._modifiedOn.Equals( _modifiedOn ) && Equals( other._note, _note );
        }

        public override bool Equals( object obj )
        {
            if ( ReferenceEquals( null, obj ) ) return false;
            if ( ReferenceEquals( this, obj ) ) return true;
            if ( obj.GetType( ) != typeof ( PasswordDigestDocument ) ) return false;
            return Equals( (PasswordDigestDocument) obj );
        }

        public override int GetHashCode( )
        {
            unchecked
            {
                int result = _digest.GetHashCode( );
                result = ( result * 397 ) ^ _masterPasswordId.GetHashCode( );
                result = ( result * 397 ) ^ _createdOn.GetHashCode( );
                result = ( result * 397 ) ^ _modifiedOn.GetHashCode( );
                result = ( result * 397 ) ^ _note.GetHashCode( );
                return result;
            }
        }

        public static bool operator ==( PasswordDigestDocument left, PasswordDigestDocument right )
        {
            return Equals( left, right );
        }

        public static bool operator !=( PasswordDigestDocument left, PasswordDigestDocument right )
        {
            return !Equals( left, right );
        }

        private readonly PasswordDigest2 _digest;

        private readonly Guid _masterPasswordId;

        private readonly DateTime _createdOn;

        private readonly DateTime _modifiedOn;

        private readonly string _note;
    }
}