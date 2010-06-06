using System;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordSerializer : IPasswordSerializer
    {
        public const string PasswordStoreElement = "password-store";
        public const string PasswordElement = "password";
        public const string KeyElement = "key";
        public const string HashElement = "hash";
        public const string MasterPasswordIdElement = "master-password";
        public const string PasswordSettingsIdElement = "password-settings";
        public const string TimestampElement = "timestamp";
        public const string NoteElement = "note";


        public void Save( IPasswordRepository passwordRepository, TextWriter writer )
        {
            XElement root = ToXml( passwordRepository );
            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          OmitXmlDeclaration = true,
                                                          ConformanceLevel = ConformanceLevel.Document,
                                                          Indent = true,
                                                          IndentChars = "  ",
                                                      };
            using ( XmlWriter xw = XmlWriter.Create( writer, xmlWriterSettings ) )
                if ( xw != null ) root.Save( xw );
        }

        private static XElement ToXml( IPasswordRepository passwordRepository )
        {
            return new XElement( PasswordStoreElement, passwordRepository.Passwords.Select( ToXml ) );
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

        public void Load( IPasswordRepository passwordRepository, TextReader textReader )
        {
            XElement xml = XElement.Load( textReader );
            ReadFromXml( passwordRepository, xml );
        }

        private static void ReadFromXml( IPasswordRepository passwordRepository, XElement xml )
        {
            foreach ( XElement passwordElement in xml.Elements( PasswordElement ) )
                ReadPasswordFromXml( passwordRepository, passwordElement );
        }

        private static void ReadPasswordFromXml( IPasswordRepository passwordRepository, XElement passwordElement )
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
            passwordRepository.AddOrUpdate( passwordDigest );
        }
    }
}