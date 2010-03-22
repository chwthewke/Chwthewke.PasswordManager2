using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Migration
{
    public class LegacyItemImporter
    {
        public LegacyItemImporter( IPasswordStore passwordStore, 
            IPasswordDigester passwordDigester, 
            IMasterPasswordFinder masterPasswordFinder )
        {
            _passwordStore = passwordStore;
            _masterPasswordFinder = masterPasswordFinder;
            _passwordDigester = passwordDigester;
        }

        private void Import( LegacyItem legacyItem, SecureString masterPassword, Guid masterPasswordId )
        {
            IPasswordGenerator generator = legacyItem.IsAlphanumeric
                                               ? PasswordGenerators.AlphaNumeric
                                               : PasswordGenerators.Full;
            string password = generator.MakePassword( legacyItem.Key, masterPassword );
            PasswordDigest importedDigest = _passwordDigester.Digest( legacyItem.Key, password, masterPasswordId, generator.Id, "Imported from v1" );
            _passwordStore.AddOrUpdate( importedDigest );
        }

        public void Import( IEnumerable<LegacyItem> items, SecureString masterPassword )
        {
            Guid masterPasswordId = _masterPasswordFinder.IdentifyMasterPassword( masterPassword ) ?? Guid.NewGuid( );
            foreach ( LegacyItem legacyItem in items )
            {
                Import( legacyItem, masterPassword, masterPasswordId );
            }
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordDigester _passwordDigester;
        private readonly IMasterPasswordFinder _masterPasswordFinder;
    }
}