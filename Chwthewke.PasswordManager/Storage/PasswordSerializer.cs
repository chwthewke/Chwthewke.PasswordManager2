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
        public const int Version = 1;

        public const string PasswordStoreElement = "password-store";
        public const string VersionAttribute = "version";
        public const string PasswordElement = "password";
        public const string KeyElement = "key";
        public const string HashElement = "hash";
        public const string MasterPasswordIdElement = "master-password";
        public const string PasswordSettingsIdElement = "password-settings";
        public const string TimestampElement = "timestamp";
        public const string NoteElement = "note";
        public const string ModifiedElement = "modified";

        public void Save( IEnumerable<PasswordDigest> passwordDigests, IPasswordStore store )
        {
            Save( passwordDigests, store.OpenWriter );
        }

        public IEnumerable<PasswordDigest> Load( IPasswordStore store )
        {
            return Load( store.OpenReader );
        }

        private void Save( IEnumerable<PasswordDigest> passwordDigests, Func<TextWriter> openWriter )
        {
            XElement root = new XElement( PasswordStoreElement, new XAttribute( VersionAttribute, Version ), passwordDigests.Select( ToXml ) );

            XmlWriterSettings xmlWriterSettings = new XmlWriterSettings
                                                      {
                                                          OmitXmlDeclaration = true,
                                                          ConformanceLevel = ConformanceLevel.Document,
                                                          Indent = true,
                                                          IndentChars = "  ",
                                                      };

            using ( TextWriter textWriter = openWriter( ) )
            using ( XmlWriter xmlWriter = XmlWriter.Create( textWriter, xmlWriterSettings ) )
                root.Save( xmlWriter );
        }

        private static XElement ToXml( PasswordDigest password )
        {
            var xElement = new XElement( PasswordElement,
                                         new XElement( KeyElement, password.Key ),
                                         new XElement( HashElement, Convert.ToBase64String( password.Hash ) ),
                                         new XElement( MasterPasswordIdElement, password.MasterPasswordId.ToString( ) ),
                                         new XElement( PasswordSettingsIdElement,
                                                       password.PasswordGeneratorId.ToString( ) ),
                                         new XElement( TimestampElement, password.CreationTime.Ticks ),
                                         new XElement( ModifiedElement, password.ModificationTime.Ticks ) );
            if ( password.Note != null )
                xElement.Add( new XElement( NoteElement, password.Note ) );
            return xElement;
        }


        private IEnumerable<PasswordDigest> Load( Func<TextReader> openReader )
        {
            try
            {
                return ReadPasswords( openReader ).ToList( );
            }
            catch ( XmlException )
            {
                return Enumerable.Empty<PasswordDigest>( );
            }
        }

        private IEnumerable<PasswordDigest> ReadPasswords( Func<TextReader> openReader )
        {
            using ( TextReader textReader = openReader( ) )
            {
                XElement root = XElement.Load( textReader );
                int version = ExtractVersion( root );


                foreach ( XElement passwordElement in root.Elements( PasswordElement ) )
                {
                    if ( !VerifyRequirements( version, passwordElement,
                                              new Requirement { ElementName = KeyElement, StartingVersion = 0 },
                                              new Requirement { ElementName = HashElement, StartingVersion = 0 },
                                              new Requirement { ElementName = MasterPasswordIdElement, StartingVersion = 0 },
                                              new Requirement { ElementName = PasswordSettingsIdElement, StartingVersion = 0 },
                                              new Requirement { ElementName = TimestampElement, StartingVersion = 0 },
                                              new Requirement { ElementName = ModifiedElement, StartingVersion = 1 } ) )

                        continue;

                    string key = ExtractFromElement( passwordElement, KeyElement, x => x );
                    byte[ ] hash = ExtractFromElement( passwordElement, HashElement, Convert.FromBase64String );
                    Guid masterPasswordId = ExtractFromElement( passwordElement, MasterPasswordIdElement, Guid.Parse );
                    Guid passwordSettingsId = ExtractFromElement( passwordElement, PasswordSettingsIdElement, Guid.Parse );
                    DateTime creationDate = ExtractFromElement( passwordElement, TimestampElement, ExtractDateTime );
                    DateTime modificationDate = ExtractFromElement( passwordElement, ModifiedElement, ExtractDateTime );
                    string note = ExtractFromElement( passwordElement, NoteElement, x => x );

                    yield return new PasswordDigest( key, hash, masterPasswordId, passwordSettingsId, creationDate, modificationDate, note );
                }
            }
        }

        private DateTime ExtractDateTime( string x )
        {
            return x == null ? new DateTime( ) : new DateTime( long.Parse( x ) );
        }

        private bool VerifyRequirements( int version, XElement passwordElement, params Requirement[ ] requirements )
        {
            return requirements.All( r => r.Check( passwordElement, version ) );
        }

        private static int ExtractVersion( XElement root )
        {
            XAttribute versionText = root.Attribute( VersionAttribute );
            int version = versionText == null ? 0 : int.Parse( versionText.Value );
            return version;
        }

        private T ExtractFromElement<T>( XElement parent, string childName, Func<string, T> extractor )
        {
            XElement childElement = parent.Element( childName );
            return extractor( childElement == null ? null : childElement.Value );
        }
    }

    internal class Requirement
    {
        public int StartingVersion { get; set; }
        public string ElementName { get; set; }

        public bool Check( XElement target, int version )
        {
            if ( StartingVersion > version )
                return true;
            return target.Element( ElementName ) != null;
        }
    }
}