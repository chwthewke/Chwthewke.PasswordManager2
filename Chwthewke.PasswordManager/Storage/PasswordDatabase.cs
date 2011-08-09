using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml.Linq;

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

        public PasswordDatabase( IPasswordStore source )
        {
            Source = source;
        }


        public IDictionary<string, PasswordDigest> Passwords
        {
            get { return _passwords; }
        }

        public IPasswordStore Source
        {
            get { return _source; }
            set { _source = value; }
        }


        private void Init( )
        {
        }

        private IEnumerable<PasswordDigest> ReadPasswords()
        {
            using ( TextReader textReader = Source.OpenReader( ) )
            {
                XElement root = XElement.Load( textReader );
                XAttribute versionText = root.Attribute( VersionAttribute );
                int version = versionText == null ? 0 : int.Parse( versionText.Value );

                // TODO not extremely readable, or modular...

                return from passwordElement in root.Elements( PasswordElement )
                       let key = passwordElement.Element( KeyElement )
                       let hash = passwordElement.Element( HashElement )
                       let masterPasswordId = passwordElement.Element( MasterPasswordIdElement )
                       let passwordSettingsId = passwordElement.Element( PasswordSettingsIdElement )
                       let timestamp = passwordElement.Element( TimestampElement )
                       let modifier = passwordElement.Element( ModifiedElement )
                       let note = passwordElement.Element( NoteElement )
                       where key != null && hash != null && masterPasswordId != null && passwordSettingsId != null &&
                             timestamp != null && (version < 1 || modifier != null)
                       select new PasswordDigest( key.Value,
                                                  Convert.FromBase64String( hash.Value ),
                                                  Guid.Parse( masterPasswordId.Value ),
                                                  Guid.Parse( passwordSettingsId.Value ),
                                                  new DateTime( long.Parse( timestamp.Value ) ),
                                                  modifier == null ? new DateTime() : new DateTime(long.Parse( modifier.Value )),
                                                  note == null ? null : note.Value );

            }
            
        }

        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );

        private IPasswordStore _source;
    }

}