using System;
using System.Collections.Generic;
using System.IO;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.Migration
{
    public class LegacyItemImporter : ILegacyItemImporter
    {
        public LegacyItemImporter( IPasswordStore passwordStore,
                                   IPasswordDigester passwordDigester,
                                   IMasterPasswordFinder masterPasswordFinder,
                                   IPasswordStoreSerializer serializer )
        {
            _passwordStore = passwordStore;
            _serializer = serializer;
            _masterPasswordFinder = masterPasswordFinder;
            _passwordDigester = passwordDigester;
        }

        public int NumPasswords
        {
            get { return _passwordStore.Passwords.Count( ); }
        }

        public IEnumerable<string> PasswordKeys
        {
            get { return _passwordStore.Passwords.Select( p => p.Key ); }
        }

        private void Import( LegacyItem legacyItem, SecureString masterPassword, Guid masterPasswordId )
        {
            IPasswordGenerator generator = legacyItem.IsAlphanumeric
                                               ? PasswordGenerators.AlphaNumeric
                                               : PasswordGenerators.Full;
            string password = generator.MakePassword( legacyItem.Key, masterPassword );
            PasswordDigest importedDigest = _passwordDigester.Digest( legacyItem.Key, password, masterPasswordId,
                                                                      generator.Id, "Imported from v1" );
            _passwordStore.AddOrUpdate( importedDigest );
        }

        public void Import( IEnumerable<LegacyItem> items, SecureString masterPassword )
        {
            Guid masterPasswordId = _masterPasswordFinder.IdentifyMasterPassword( masterPassword ) ?? Guid.NewGuid( );
            foreach ( LegacyItem legacyItem in items )
                Import( legacyItem, masterPassword, masterPasswordId );
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordDigester _passwordDigester;
        private readonly IMasterPasswordFinder _masterPasswordFinder;
        private readonly IPasswordStoreSerializer _serializer;

        public void Save( string fileName )
        {
            using ( FileStream outputStream = File.OpenWrite( fileName ) )
                _serializer.Save( _passwordStore, outputStream );
        }
    }
}