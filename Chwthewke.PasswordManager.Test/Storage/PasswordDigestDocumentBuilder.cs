using System;
using System.Xml.Linq;
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

        public PasswordDigest Digest
        {
            get { return new PasswordDigest( Key, Hash, Iteration, PasswordGenerator ); }
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
            Hash = new byte[ ] { 0x00 };
            Note = string.Empty;
            Iteration = 1;
            PasswordGenerator = PasswordGenerators.LegacyAlphaNumeric;
        }

        public static implicit operator PasswordDigestDocument( PasswordDigestDocumentBuilder builder )
        {
            return builder.Build( );
        }

        public PasswordDigestDocument Build( )
        {
            return new PasswordDigestDocument( Digest, MasterPasswordId, CreatedOn, ModifiedOn, Note );
        }

        public XElement ToXml( )
        {
            return new XElement( PasswordSerializer.PasswordElement,
                                 new XElement( PasswordSerializer.KeyElement, Key ),
                                 new XElement( PasswordSerializer.HashElement,
                                               Convert.ToBase64String( Hash ) ),
                                 new XElement( PasswordSerializer.PasswordSettingsIdElement,
                                               PasswordGenerator.ToString( ) ),
                                 new XElement( PasswordSerializer.IterationElement,
                                               Iteration.ToString( ) ),
                                 new XElement( PasswordSerializer.MasterPasswordIdElement,
                                               MasterPasswordId.ToString( ) ),
                                 new XElement( PasswordSerializer.TimestampElement,
                                               CreatedOn.Ticks ),
                                 new XElement( PasswordSerializer.ModifiedElement,
                                               ModifiedOn.Ticks ),
                                 new XElement( PasswordSerializer.NoteElement,
                                               Note ) );
        }

        public static implicit operator XElement( PasswordDigestDocumentBuilder builder )
        {
            return builder.ToXml( );
        }
    }
}