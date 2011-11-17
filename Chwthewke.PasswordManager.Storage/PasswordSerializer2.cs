using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordSerializer2
    {
        public const int Version = 2;

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
        public const string IterationElement = "iteration";

        public void Save( IEnumerable<PasswordDigestDocument> passwordDigests, ITextResource store )
        {
            Save( passwordDigests, store.OpenWriter );
        }

        public IEnumerable<PasswordDigestDocument> Load( ITextResource store )
        {
            return Load( store.OpenReader );
        }

        private void Save( IEnumerable<PasswordDigestDocument> passwordDigests, Func<TextWriter> openWriter )
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

        private static XElement ToXml( PasswordDigestDocument password )
        {
            var xElement = new XElement( PasswordElement,
                                         new XElement( KeyElement, password.Digest.Key ),
                                         new XElement( HashElement, Convert.ToBase64String( password.Digest.Hash ) ),
                                         new XElement( MasterPasswordIdElement, password.MasterPasswordId.ToString( ) ),
                                         new XElement( PasswordSettingsIdElement,
                                                       password.Digest.PasswordGenerator.ToString( ) ),
                                         new XElement( IterationElement, password.Digest.Iteration ),
                                         new XElement( TimestampElement, password.CreatedOn.Ticks ),
                                         new XElement( ModifiedElement, password.ModifiedOn.Ticks ) );
            if ( password.Note != null )
                xElement.Add( new XElement( NoteElement, password.Note ) );
            return xElement;
        }


        private IEnumerable<PasswordDigestDocument> Load( Func<TextReader> openReader )
        {
            try
            {
                return ReadPasswords( openReader ).ToList( );
            }
            catch ( XmlException )
            {
                return Enumerable.Empty<PasswordDigestDocument>( );
            }
        }

        private IEnumerable<PasswordDigestDocument> ReadPasswords( Func<TextReader> openReader )
        {
            using ( TextReader textReader = openReader( ) )
            {
                XElement root = XElement.Load( textReader );
                int version = ExtractVersion( root );


                foreach ( XElement passwordElement in root.Elements( PasswordElement ) )
                {
                    if ( !VerifyRequirements( version, passwordElement,
                                              new SerializerRequirement { ElementName = KeyElement, StartingVersion = 0 },
                                              new SerializerRequirement { ElementName = HashElement, StartingVersion = 0 },
                                              new SerializerRequirement { ElementName = MasterPasswordIdElement, StartingVersion = 0 },
                                              new SerializerRequirement { ElementName = PasswordSettingsIdElement, StartingVersion = 0 },
                                              new SerializerRequirement { ElementName = TimestampElement, StartingVersion = 0 },
                                              new SerializerRequirement { ElementName = ModifiedElement, StartingVersion = 1 },
                                              new SerializerRequirement { ElementName = IterationElement, StartingVersion = 2 } ) )

                        continue;

                    string key = ExtractFromElement( passwordElement, KeyElement, x => x );
                    byte[ ] hash = ExtractFromElement( passwordElement, HashElement, Convert.FromBase64String );
                    Guid masterPasswordId = ExtractFromElement( passwordElement, MasterPasswordIdElement, Guid.Parse );
                    Guid passwordSettingsId = ExtractFromElement( passwordElement, PasswordSettingsIdElement, Guid.Parse );
                    DateTime creationDate = ExtractFromElement( passwordElement, TimestampElement, ExtractCreationDate );
                    DateTime modificationDate = ExtractFromElement( passwordElement, ModifiedElement, s => ExtractModificationDate( s, creationDate ) );
                    string note = ExtractFromElement( passwordElement, NoteElement, x => x );
                    int iteration = ExtractFromElement( passwordElement, IterationElement, ExtractIteration );

                    yield return
                        new PasswordDigestDocument( new PasswordDigest( key, hash, iteration, passwordSettingsId ),
                                                    masterPasswordId, creationDate, modificationDate, note );
                }
            }
        }

        private int ExtractIteration( string s )
        {
            // TODO unit test coercion
            return string.IsNullOrEmpty( s ) ? 1 : Math.Max( 1, int.Parse( s ) );
        }

        private DateTime ExtractCreationDate( string s )
        {
            return ExtractDateTime( s, DateTime.Now );
        }

        private DateTime ExtractModificationDate( string s, DateTime creationDate )
        {
            DateTime modificationDate = ExtractDateTime( s, creationDate );
            return modificationDate.CompareTo( creationDate ) < 0 ? creationDate : modificationDate;
        }

        private DateTime ExtractDateTime( string x, DateTime defaultValue )
        {
            return x == null ? defaultValue : new DateTime( long.Parse( x ) );
        }

        private bool VerifyRequirements( int version, XElement passwordElement, params SerializerRequirement[ ] serializerRequirements )
        {
            return serializerRequirements.All( r => r.Check( passwordElement, version ) );
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
}