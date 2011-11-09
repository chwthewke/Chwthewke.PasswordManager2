using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordExchange : IPasswordExchange
    {
        public PasswordExchange( IPasswordSerializer passwordSerializer, IPasswordDatabase passwordDatabase )
        {
            _passwordSerializer = passwordSerializer;
            _passwordDatabase = passwordDatabase;
        }

        // TODO possibly return a "report" to be presented to the user
        public void ImportPasswords( FileInfo externalPasswordFile )
        {
            IEnumerable<PasswordDigest> passwords = _passwordSerializer.Load( new FileTextResource( externalPasswordFile ) ).ToList( );

            IDictionary<Guid, Guid> masterPasswordRelations = new Dictionary<Guid, Guid>( );
            IList<PasswordDigest> toImport = new List<PasswordDigest>( );
            foreach ( PasswordDigest passwordDigest in passwords )
            {
                PasswordDigest currentDigest = _passwordDatabase.FindByKey( passwordDigest.Key );
                if ( currentDigest == null )
                    toImport.Add( passwordDigest );
                else if ( currentDigest.Hash.SequenceEqual( passwordDigest.Hash ) )
                    masterPasswordRelations[ passwordDigest.MasterPasswordId ] = currentDigest.MasterPasswordId;
            }

            foreach ( PasswordDigest passwordDigest in toImport )
            {
                if ( !masterPasswordRelations.ContainsKey( passwordDigest.MasterPasswordId ) )
                    _passwordDatabase.AddOrUpdate( passwordDigest );
                else
                {
                    PasswordDigest fixedCopy = new PasswordDigest( passwordDigest.Key,
                                                                   passwordDigest.Hash,
                                                                   masterPasswordRelations[ passwordDigest.MasterPasswordId ],
                                                                   passwordDigest.PasswordGeneratorId,
                                                                   passwordDigest.CreationTime,
                                                                   passwordDigest.ModificationTime,
                                                                   passwordDigest.Iteration,
                                                                   passwordDigest.Note );
                    _passwordDatabase.AddOrUpdate( fixedCopy );
                }
            }
        }

        public void ExportPasswords( FileInfo targetFile )
        {
            _passwordSerializer.Save( _passwordDatabase.Passwords, new FileTextResource( targetFile ) );
        }

        private readonly IPasswordSerializer _passwordSerializer;
        private readonly IPasswordDatabase _passwordDatabase;
    }
}