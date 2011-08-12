using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class PasswordDigestBuilder
    {
        public string Key { get; set; }

        public byte[] Hash { get; set; }

        public Guid MasterPasswordId { get; set; }

        public Guid PasswordGeneratorId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ModificationTime { get; set; }

        public string Note { get; set; }

        public PasswordDigestBuilder( )
        {
            Key = "key";
            Hash = new byte[] {};
            Note = string.Empty;
        }

        public PasswordDigestBuilder WithKey( string key )
        {
            Key = key;
            return this;
        }

        public PasswordDigestBuilder WithHash( byte[] hash )
        {
            Hash = hash;
            return this;
        }

        public PasswordDigestBuilder WithMasterPasswordId( Guid masterPasswordId )
        {
            MasterPasswordId = masterPasswordId;
            return this;
        }

        public PasswordDigestBuilder WithGeneratorId( Guid generatorId )
        {
            PasswordGeneratorId = generatorId;
            return this;
        }

        public PasswordDigestBuilder WithCreationTime( DateTime creationTime )
        {
            CreationTime = creationTime;
            return this;
        }

        public PasswordDigestBuilder WithNote( string note )
        {
            Note = note;
            return this;
        }

        public static implicit operator PasswordDigest( PasswordDigestBuilder builder )
        {
            return builder.Build( );
        }

        private PasswordDigest Build( )
        {
            return new PasswordDigest( Key, Hash, MasterPasswordId, PasswordGeneratorId, CreationTime, ModificationTime,
                                       Note );
        }
    }
}