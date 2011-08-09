using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Linq;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordDatabase
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
        public const string ModifiedElement = "modified";
        public const string NoteElement = "note";
        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );

        public PasswordDatabase( IPasswordStore source )
        {
            Source = source;
        }


        public IDictionary<string, PasswordDigest> Passwords
        {
            get { return _passwords; }
        }

        public IPasswordStore Source { get; set; }


        private void Init( )
        {
        }

        private IEnumerable<PasswordDigest> ReadPasswords( )
        {
            using ( TextReader textReader = Source.OpenReader( ) )
            {
                XElement root = XElement.Load( textReader );
                int version = ExtractVersion( root );

                // TODO not extremely readable, or modular...

                foreach ( XElement passwordElement in root.Elements( PasswordElement ) )
                {
                    if (!VerifyRequirements( version, passwordElement, 
                        new Requirement { ElementName = KeyElement, StartingVersion = 0 },
                        new Requirement { ElementName = HashElement, StartingVersion = 0 },
                        new Requirement { ElementName = MasterPasswordIdElement, StartingVersion = 0 },
                        new Requirement { ElementName = PasswordSettingsIdElement, StartingVersion = 0 },
                        new Requirement { ElementName = TimestampElement, StartingVersion = 0 },
                        new Requirement { ElementName = ModifiedElement, StartingVersion = 1 }))
                        
                        continue;

/*
                    string key = ExtractKey( passwordElement );
                    byte[] hash = ExtractHash( passwordElement );
                    Guid masterPasswordId = ExtractGuid( passwordElement, MasterPasswordIdElement );
                    Guid passwordSettingsId = ExtractGuid( passwordElement, PasswordSettingsIdElement );


                    XElement timestamp = passwordElement.Element( TimestampElement );
                    XElement modifier = passwordElement.Element( ModifiedElement );
                    XElement note = passwordElement.Element( NoteElement );

                    if ( key != null && hash != null && masterPasswordId != null && passwordSettingsId != null &&
                         timestamp != null && ( version < 1 || modifier != null ) )
                        yield return
                            new PasswordDigest( key.Value, hash.Value ) ,
                    Guid.Parse( masterPasswordId.Value ),
                    Guid.Parse( passwordSettingsId.Value ),
                    new DateTime( long.Parse( timestamp.Value ) ),
                    modifier == null
                        ? new DateTime( )
                        : new DateTime( long.Parse( modifier.Value ) ),
                    note == null ? null : note.Value)
*/
                    yield return null;
                }
            }
        }

        private bool VerifyRequirements( int version, XElement passwordElement, params Requirement[] requirements)
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
            return childElement == null ? default( T ) : extractor( childElement.Value );
        }

        private static string ExtractKey( XElement passwordElement )
        {
            return ExtractStringOrNull( passwordElement, KeyElement );
        }

        private static byte[] ExtractHash( XElement passwordElement )
        {
            string hashString = ExtractStringOrNull( passwordElement, HashElement );
            return hashString == null ? null : Convert.FromBase64String( hashString );
        }

        private static Guid ExtractGuid( XElement passwordElement, string guidElement )
        {
            return Guid.Parse( ExtractStringOrNull( passwordElement, guidElement ) );
        }

        private static DateTime ExtractDateTime( XElement passwordElement, string dateTimeElement )
        {
            string dateTimeString = ExtractStringOrNull( passwordElement, TimestampElement );
            return dateTimeString == null ? default(DateTime) : new DateTime( long.Parse( dateTimeString ) );
        }

        private static string ExtractStringOrNull( XElement passwordElement, string childElement )
        {
            XElement child = passwordElement.Element( childElement );
            return child == null ? null : child.Value;
        }
    }


    internal class Requirement
    {

        public int StartingVersion { get; set; }
        public string ElementName { get; set; }

        public bool Check( XElement target, int version )
        {
            if (StartingVersion > version)
                return true;
            return target.Element( ElementName ) != null;
        }
    }
}