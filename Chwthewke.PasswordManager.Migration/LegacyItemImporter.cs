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
        public LegacyItemImporter( IPasswordRepository passwordRepository,
                                   IPasswordDigester passwordDigester,
                                   IPasswordSerializer serializer )
        {
            _passwordRepository = passwordRepository;
            _serializer = serializer;
            _passwordDigester = passwordDigester;
        }

        public int NumPasswords
        {
            get { return _passwordRepository.Passwords.Count( ); }
        }

        public IEnumerable<string> PasswordKeys
        {
            get { return _passwordRepository.Passwords.Select( p => p.Key ); }
        }

        private void Import( LegacyItem legacyItem, SecureString masterPassword, Guid masterPasswordId )
        {
            IPasswordGenerator generator = legacyItem.IsAlphanumeric
                                               ? PasswordGenerators.AlphaNumeric
                                               : PasswordGenerators.Full;
            string password = generator.MakePassword( legacyItem.Key, masterPassword );
            PasswordDigest importedDigest = _passwordDigester.Digest( legacyItem.Key, password, masterPasswordId,
                                                                      generator.Id, "Imported from v1" );
            _passwordRepository.AddOrUpdate( importedDigest );
        }

        public void Import( IEnumerable<LegacyItem> items, SecureString masterPassword )
        {
            Guid masterPasswordId = _passwordRepository.IdentifyMasterPassword( masterPassword ) ?? Guid.NewGuid( );
            foreach ( LegacyItem legacyItem in items )
                Import( legacyItem, masterPassword, masterPasswordId );
        }

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordDigester _passwordDigester;
        private readonly IPasswordSerializer _serializer;

        public void Save( string fileName )
        {
            using ( TextWriter writer = new StreamWriter( File.OpenWrite( fileName ) ) )
                _serializer.Save( _passwordRepository.Passwords, writer );
        }

        public override string ToString( )
        {
            TextWriter w = new StringWriter( );
            _serializer.Save( _passwordRepository.Passwords, w );
            return w.ToString( );
        }
    }
}