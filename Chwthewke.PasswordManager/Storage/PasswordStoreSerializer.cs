using System;
using System.IO;
using System.Xml;
using System.Xml.Linq;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordStoreSerializer : IPasswordStoreSerializer
    {
        public const string PasswordStoreElement = "password-store";
        public const string PasswordElement = "password";
        public const string KeyElement = "key";
        public const string HashElement = "hash";
        public const string MasterPasswordIdElement = "master-password";
        public const string PasswordSettingsIdElement = "password-settings";
        public const string TimestampElement = "timestamp";
        public const string NoteElement = "note";


        public void Save( IPasswordStore passwordStore, TextWriter writer )
        {
            XElement root = ToXml( passwordStore );
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

        public void Load( IPasswordStore passwordStore, TextReader textReader )
        {
            XElement xml = XElement.Load( textReader );
            ReadFromXml( passwordStore, xml );
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

    }
}