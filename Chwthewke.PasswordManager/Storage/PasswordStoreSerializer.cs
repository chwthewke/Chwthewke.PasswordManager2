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
            return new XElement( PasswordElement,
                                 new XElement( KeyElement, password.Key ),
                                 new XElement( HashElement, Convert.ToBase64String( password.Hash ) ),
                                 new XElement( GuidElement, password.MasterPasswordId.ToString( ) ),
                                 new XElement( TimestampElement, password.CreationTime.Ticks ),
                                 new XElement( NoteElement, password.Note ) );
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
            XElement base64Hash = passwordElement.Element( HashElement );
            if ( key == null || base64Hash == null )
                return;

            PasswordInfo passwordInfo = new PasswordInfo( key.Value,
                                                          Convert.FromBase64String( base64Hash.Value ),
                                                          default( Guid ), default( DateTime ),
                                                          default( string ) );
            passwordStore.AddOrUpdate( passwordInfo );
        }

        private readonly Encoding _encoding;
    }
}