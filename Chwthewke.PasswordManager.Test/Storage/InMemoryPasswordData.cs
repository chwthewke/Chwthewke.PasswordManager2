using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class InMemoryPasswordData : IPasswordData
    {
        public InMemoryPasswordData( )
        {
            Passwords = new List<PasswordDigestDocument>( );
        }

        public IList<PasswordDigestDocument> LoadPasswords( )
        {
            return Passwords.Select( ClonePassword ).ToList( );
        }

        private PasswordDigestDocument ClonePassword( PasswordDigestDocument source )
        {
            return new PasswordDigestDocumentBuilder
                       {
                           Digest = new PasswordDigest2( source.Digest.Key,
                                                         source.Digest.Hash.Clone( ) as byte[ ],
                                                         source.Digest.Iteration,
                                                         source.Digest.PasswordGenerator ),
                           MasterPasswordId = source.MasterPasswordId,
                           CreatedOn = source.CreatedOn,
                           ModifiedOn = source.ModifiedOn,
                           Note = source.Note
                       };
        }

        public void SavePasswords( IList<PasswordDigestDocument> passwords )
        {
            Passwords = passwords.Select( ClonePassword ).ToList( );
        }

        private IList<PasswordDigestDocument> Passwords { get; set; }
    }
}