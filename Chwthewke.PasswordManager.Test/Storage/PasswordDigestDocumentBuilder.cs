using System;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class PasswordDigestDocumentBuilder
    {
        public string Key { get; set; }

        public byte[ ] Hash { get; set; }

        public Guid PasswordGenerator { get; set; }

        public int Iteration { get; set; }

        public Guid MasterPasswordId { get; set; }

        public DateTime CreatedOn { get; set; }

        public DateTime ModifiedOn { get; set; }

        public string Note { get; set; }

        public PasswordDigest2 Digest
        {
            get { return new PasswordDigest2( Key, Hash, Iteration, PasswordGenerator ); }
            set
            {
                Key = value.Key;
                Hash = value.Hash;
                PasswordGenerator = value.PasswordGenerator;
                Iteration = value.Iteration;
            }
        }

        public PasswordDigestDocumentBuilder( )
        {
            Key = "key";
            Hash = new byte[ ] { };
            Note = string.Empty;
        }

        public static implicit operator PasswordDigestDocument( PasswordDigestDocumentBuilder builder )
        {
            return builder.Build( );
        }

        public PasswordDigestDocument Build( )
        {
            return new PasswordDigestDocument( Digest, MasterPasswordId, CreatedOn, ModifiedOn, Note );
        }
    }
}