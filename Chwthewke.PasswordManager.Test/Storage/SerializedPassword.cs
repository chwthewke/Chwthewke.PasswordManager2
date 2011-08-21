using System;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    // TODO factor with PasswordDigestBuilder
    public class SerializedPassword
    {
        public SerializedPassword( string key )
        {
            _key = key;
            Hash = new byte[ 0 ];
        }

        public string Key
        {
            get { return _key; }
        }

        public byte[ ] Hash { get; set; }

        public Guid MasterPasswordId { get; set; }

        public Guid PasswordSettingsId { get; set; }

        public DateTime CreationTime { get; set; }

        public DateTime ModificationTime { get; set; }

        public string Note { get; set; }

        public static explicit operator XElement( SerializedPassword builder )
        {
            var xElement = new XElement( PasswordSerializer.PasswordElement,
                                         new XElement( PasswordSerializer.KeyElement, builder.Key ),
                                         new XElement( PasswordSerializer.HashElement,
                                                       Convert.ToBase64String( builder.Hash ) ),
                                         new XElement( PasswordSerializer.MasterPasswordIdElement,
                                                       builder.MasterPasswordId.ToString( ) ),
                                         new XElement( PasswordSerializer.PasswordSettingsIdElement,
                                                       builder.PasswordSettingsId.ToString( ) ),
                                         new XElement( PasswordSerializer.TimestampElement,
                                                       builder.CreationTime.Ticks ),
                                         new XElement( PasswordSerializer.ModifiedElement,
                                                       builder.ModificationTime.Ticks ) );
            if ( builder.Note != null )
                xElement.Add( new XElement( PasswordSerializer.NoteElement, builder.Note ) );

            return xElement;
        }

        private readonly string _key;
    }
}