using System;
using System.IO;
using System.Text;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordStoreSerializer : IPasswordStoreSerializer
    {
        public const string PasswordStoreElement = "password-store";
        public const string PasswordElement = "password";
        public const string KeyElement = "key";
        public const string HashElement = "hash";
        public const string GuidElement = "guid";
        public const string TimestampElement = "timestamp";
        public const string NoteElement = "note";

        public PasswordStoreSerializer( Encoding encoding )
        {
            _encoding = encoding;
        }

        public void Save( IPasswordStore passwordStore, Stream outputStream )
        {
            XElement root = ToXml( passwordStore );
            using (
                XmlWriter xw = XmlWriter.Create( outputStream,
                                                 new XmlWriterSettings
                                                     { OmitXmlDeclaration = true, Encoding = _encoding } ) )
                root.Save( xw );
        }

        private static XElement ToXml( IPasswordStore passwordStore )
        {
            return new XElement( PasswordStoreElement, passwordStore.Passwords.Select( ToXml ) );
        }

        private static XElement ToXml( PasswordInfo password )
        {
            var xElement = new XElement( PasswordElement,
                                         new XElement( KeyElement, password.Key ),
                                         new XElement( HashElement, Convert.ToBase64String( password.Hash ) ),
                                         new XElement( GuidElement, password.MasterPasswordId.ToString( ) ),
                                         new XElement( TimestampElement, password.CreationTime.Ticks ) );
            if ( password.Note != null )
                xElement.Add( new XElement( NoteElement, password.Note ) );
            return xElement;
        }

        public void Load( IPasswordStore passwordStore, Stream inputStream )
        {
            using ( TextReader tr = new StreamReader( inputStream, _encoding ) )
            {
                XElement xml = XElement.Load( tr );
                ReadFromXml( passwordStore, xml );
            }
        }

        private static void ReadFromXml( IPasswordStore passwordStore, XElement xml )
        {
            foreach ( XElement passwordElement in xml.Elements( PasswordElement ) )
                ReadPasswordFromXml( passwordStore, passwordElement );
        }

        private static void ReadPasswordFromXml( IPasswordStore passwordStore, XElement passwordElement )
        {
            XElement key = passwordElement.Element( KeyElement );
            XElement hash = passwordElement.Element( HashElement );
            XElement guid = passwordElement.Element( GuidElement );
            XElement timestamp = passwordElement.Element( TimestampElement );
            XElement note = passwordElement.Element( NoteElement );

            if ( key == null || hash == null || guid == null || timestamp == null )
                return;

            PasswordInfo passwordInfo = new PasswordInfo( key.Value,
                                                          Convert.FromBase64String( hash.Value ),
                                                          Guid.Parse( guid.Value ),
                                                          new DateTime( long.Parse( timestamp.Value ) ),
                                                          note == null ? null : note.Value );
            passwordStore.AddOrUpdate( passwordInfo );
        }

        private readonly Encoding _encoding;
    }
}