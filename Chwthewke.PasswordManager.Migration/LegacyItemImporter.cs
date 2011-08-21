using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Migration
{
    public class LegacyItemImporter : ILegacyItemImporter
    {
        public LegacyItemImporter( IPasswordDatabase passwordDatabase,
                                   IMasterPasswordMatcher masterPasswordMatcher,
                                   IPasswordDigester passwordDigester,
                                   IPasswordSerializer serializer )
        {
            _passwordDatabase = passwordDatabase;
            _masterPasswordMatcher = masterPasswordMatcher;
            _serializer = serializer;
            _passwordDigester = passwordDigester;
        }

        public int NumPasswords
        {
            get { return _passwordDatabase.Passwords.Count( ); }
        }

        public IEnumerable<string> PasswordKeys
        {
            get { return _passwordDatabase.Passwords.Select( p => p.Key ); }
        }

        private void Import( LegacyItem legacyItem, SecureString masterPassword, Guid masterPasswordId )
        {
            IPasswordGenerator generator = legacyItem.IsAlphanumeric
                                               ? PasswordGenerators.AlphaNumeric
                                               : PasswordGenerators.Full;
            string password = generator.MakePassword( legacyItem.Key, masterPassword );
            PasswordDigest importedDigest = _passwordDigester.Digest( legacyItem.Key, password, masterPasswordId,
                                                                      generator.Id, null, "Imported from v1" );
            _passwordDatabase.AddOrUpdate( importedDigest );
        }

        public void Import( IEnumerable<LegacyItem> items, SecureString masterPassword )
        {
            Guid masterPasswordId = _masterPasswordMatcher.IdentifyMasterPassword( masterPassword ) ?? Guid.NewGuid( );
            foreach ( LegacyItem legacyItem in items )
                Import( legacyItem, masterPassword, masterPasswordId );
        }

        private readonly IPasswordDatabase _passwordDatabase;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IPasswordDigester _passwordDigester;
        private readonly IPasswordSerializer _serializer;

        public void Save( string fileName )
        {
            _serializer.Save( _passwordDatabase.Passwords, new FilePasswordStore( new FileInfo( fileName ) ) );
        }
    }
}