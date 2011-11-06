using System;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordDigestDocument
    {
        public PasswordDigestDocument( PasswordDigest2 digest, Guid masterPasswordId, DateTime createdOn, DateTime modifiedOn, string note )
        {
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

        public PasswordDigestDocument Update( PasswordDigest2 newDigest, Guid newMasterPasswordId, DateTime updatedOn, string newNote )
        {
            return new PasswordDigestDocument( newDigest, newMasterPasswordId, _createdOn, updatedOn, newNote );
        }

        public PasswordDigestDocument Delete( DateTime deletedOn )
        {
            return new PasswordDigestDocument( new PasswordDigest2( _digest.Key, new byte[ 0 ], 0, default( Guid ) ),
                                               default( Guid ), _createdOn, deletedOn, string.Empty );
        }

        private readonly PasswordDigest2 _digest;

        private readonly Guid _masterPasswordId;

        private readonly DateTime _createdOn;

        private readonly DateTime _modifiedOn;

        private readonly string _note;
    }
}