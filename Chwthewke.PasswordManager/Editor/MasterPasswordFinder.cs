using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public class MasterPasswordFinder : IMasterPasswordFinder
    {
        public MasterPasswordFinder( IPasswordStore store, IMasterPasswordMatcher matcher )
        {
            if ( store == null )
                throw new ArgumentNullException( "store" );

            if ( matcher == null )
                throw new ArgumentNullException( "matcher" );

            _store = store;
            _matcher = matcher;
        }

        public Guid? IdentifyMasterPassword( SecureString masterPassword )
        {
            IEnumerable<PasswordDigest> candidates =
                _store.Passwords.GroupBy( pw => pw.MasterPasswordId ).Select( g => g.First( ) );
            PasswordDigest matchingDigest =
                candidates.FirstOrDefault( dig => _matcher.MatchMasterPassword( masterPassword, dig ) );
            return matchingDigest != null ? matchingDigest.MasterPasswordId : ( Guid? ) null;
        }

        private readonly IMasterPasswordMatcher _matcher;
        private readonly IPasswordStore _store;
    }
}