using System;
using System.Collections.Generic;
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

        [ Obsolete ]
        public void Save( IPasswordRepository passwordRepository, TextWriter writer )
        {
            IEnumerable<PasswordDigest> passwordDigests = passwordRepository.Passwords;
            Save( passwordDigests, writer );
        }

        public void Save( IEnumerable<PasswordDigest> passwordDigests, TextWriter writer )
        {
            XElement root = new XElement( PasswordStoreElement, passwordDigests.Select( ToXml ) );
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

        public IEnumerable<PasswordDigest> Load( TextReader textReader )
        {
            return ReadFromXml( XElement.Load( textReader ) );
        }

        private static IEnumerable<PasswordDigest> ReadFromXml( XElement xml )
        {
            return from passwordElement in xml.Elements( PasswordElement )
                   let key = passwordElement.Element( KeyElement )
                   let hash = passwordElement.Element( HashElement )
                   let masterPasswordId = passwordElement.Element( MasterPasswordIdElement )
                   let passwordSettingsId = passwordElement.Element( PasswordSettingsIdElement )
                   let timestamp = passwordElement.Element( TimestampElement )
                   let note = passwordElement.Element( NoteElement )
                   where key != null && hash != null && masterPasswordId != null && passwordSettingsId != null &&
                         timestamp != null
                   select new PasswordDigest( key.Value,
                                              Convert.FromBase64String( hash.Value ),
                                              Guid.Parse( masterPasswordId.Value ),
                                              Guid.Parse( passwordSettingsId.Value ),
                                              new DateTime( long.Parse( timestamp.Value ) ),
                                              note == null ? null : note.Value );
        }
    }
}