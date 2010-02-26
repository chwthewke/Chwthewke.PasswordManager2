using System;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class SerializedPassword
    {
        public SerializedPassword( string key )
        {
            _key = key;
            Hash = new byte[0];
        }

        public string Key
        {
            get { return _key; }
        }

        public byte[ ] Hash { get; set; }

        public Guid MasterPasswordId { get; set; }

        public DateTime CreationTime { get; set; }

        public string Note { get; set; }

        public static explicit operator XElement( SerializedPassword builder )
        {
            return new XElement( PasswordStoreSerializer.PasswordElement,
                                 new XElement( PasswordStoreSerializer.KeyElement, builder.Key ),
                                 new XElement( PasswordStoreSerializer.HashElement, Convert.ToBase64String( builder.Hash ) ),
                                 new XElement( PasswordStoreSerializer.GuidElement, builder.MasterPasswordId.ToString( ) ),
                                 new XElement( PasswordStoreSerializer.TimestampElement, builder.CreationTime.Ticks ),
                                 new XElement( PasswordStoreSerializer.NoteElement, builder.Note ) );
        }

        private readonly string _key;
    }
}