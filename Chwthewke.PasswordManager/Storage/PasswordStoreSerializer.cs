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
        public const string MasterPasswordIdElement = "master-password";
        public const string PasswordSettingsIdElement = "password-settings";
        public const string TimestampElement = "timestamp";
        public const string NoteElement = "note";

        public PasswordStoreSerializer( Encoding encoding )
        {
            if ( encoding == null )
                throw new ArgumentNullException( "encoding" );

            _encoding = encoding;
        }

        public void Save( IPasswordStore passwordStore, Stream outputStream )
        {
            XElement root = ToXml( passwordStore );
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          OmitXmlDeclaration = true,
                                                          Encoding = _encoding,
                                                          ConformanceLevel = ConformanceLevel.Document,
                                                          Indent = true,
                                                          IndentChars = "  ",
                                                          
                                                      };
            using ( XmlWriter xw = XmlWriter.Create( outputStream, xmlWriterSettings ) )
                root.Save( xw );
        }

        private static XElement ToXml( IPasswordStore passwordStore )
        {
            return new XElement( PasswordStoreElement, passwordStore.Passwords.Select( ToXml ) );
        }

        private static XElement ToXml( PasswordDigest password )
        {
            var xElement = new XElement( PasswordElement,
                                         new XElement( KeyElement, password.Key ),
                                         new XElement( HashElement, Convert.ToBase64String( password.Hash ) ),
                                         new XElement( MasterPasswordIdElement, password.MasterPasswordId.ToString( ) ),
                                         new XElement( PasswordSettingsIdElement,
                                                       password.PasswordGeneratorId.ToString( ) ),
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
            XElement masterPasswordId = passwordElement.Element( MasterPasswordIdElement );
            XElement passwordSettingsId = passwordElement.Element( PasswordSettingsIdElement );
            XElement timestamp = passwordElement.Element( TimestampElement );
            XElement note = passwordElement.Element( NoteElement );

            if ( key == null || hash == null || masterPasswordId == null || passwordSettingsId == null ||
                 timestamp == null )
                return;

            PasswordDigest passwordDigest = new PasswordDigest( key.Value,
                                                                Convert.FromBase64String( hash.Value ),
                                                                Guid.Parse( masterPasswordId.Value ),
                                                                Guid.Parse( passwordSettingsId.Value ),
                                                                new DateTime( long.Parse( timestamp.Value ) ),
                                                                note == null ? null : note.Value );
            passwordStore.AddOrUpdate( passwordDigest );
        }

        private readonly Encoding _encoding;
    }
}