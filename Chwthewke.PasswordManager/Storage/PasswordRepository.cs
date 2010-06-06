using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordRepository : IPasswordRepository
    {
        public PasswordRepository( IEnumerable<IPasswordGenerator> generators, IHashFactory hashFactory )
        {
            _generators = generators;
            _hashFactory = hashFactory;
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

        public Guid? IdentifyMasterPassword( SecureString masterPassword )
        {
            IEnumerable<PasswordDigest> candidates =
                Passwords.GroupBy( pw => pw.MasterPasswordId ).Select( g => g.First( ) );

            PasswordDigest matchingDigest =
                candidates.FirstOrDefault( dig => MatchMasterPassword( dig, masterPassword ) );


            return matchingDigest != null ? matchingDigest.MasterPasswordId : ( Guid? ) null;
        }

        private bool MatchMasterPassword( PasswordDigest dig, SecureString masterPassword )
        {
            IPasswordGenerator generator = _generators.FirstOrDefault( g => g.Id == dig.PasswordGeneratorId );
            if ( generator == null )
                return false;
            string generatedPassword = generator.MakePassword( dig.Key, masterPassword );
            return _hashFactory.GetHash( )
                .Append( PasswordDigester.DigestSalt, Encoding.UTF8 )
                .Append( generatedPassword, Encoding.UTF8 )
                .GetValue( )
                .SequenceEqual( dig.Hash );
        }


        private readonly IEnumerable<IPasswordGenerator> _generators;
        private readonly IHashFactory _hashFactory;

        private readonly IDictionary<string, PasswordDigest> _passwords = new Dictionary<string, PasswordDigest>( );
    }
}