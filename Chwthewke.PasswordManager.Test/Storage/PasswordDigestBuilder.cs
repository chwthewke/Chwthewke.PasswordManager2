using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class PasswordDigestBuilder
    {
        public string Key { get; set; }

        public byte[ ] Hash { get; set; }

        public Guid MasterPasswordId { get; set; }

        public Guid PasswordGeneratorId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ModificationTime { get; set; }

        public string Note { get; set; }

        public PasswordDigestBuilder( )
        {
            Key = "key";
            Hash = new byte[ ] { };
            Note = string.Empty;
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