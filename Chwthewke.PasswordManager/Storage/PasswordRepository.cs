using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    // TODO extract master password guessing out, merge IPasswordRepository, IPersistenceService, part of IPasswordStore into this.

    internal class PasswordRepository : IPasswordRepository
    {


        public PasswordRepository(  )
        {
        }

        public void AddOrUpdate( PasswordDigest passwordDigest )
        {
            _passwords[ passwordDigest.Key ] = passwordDigest;
        }

        public bool Remove( string key )
        {
            return _passwords.Remove( key );
        }

        public IEnumerable<PasswordDigest> Passwords
        {
            get { return new List<PasswordDigest>( _passwords.Values ); }
        }

        public PasswordDigest FindPasswordInfo( string key )
        {
            PasswordDigest result;
            _passwords.TryGetValue( key, out result );
            return result;
        }

        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );
    }
}