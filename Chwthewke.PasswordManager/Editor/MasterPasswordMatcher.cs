using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.Editor
{
    public class MasterPasswordMatcher : IMasterPasswordMatcher
    {
        public MasterPasswordMatcher( IEnumerable<IPasswordGenerator> generators, IHashFactory hashFactory )
        {
            if ( hashFactory == null )
                throw new ArgumentNullException( "hashFactory" );

            _generators = new List<IPasswordGenerator>( generators );
            _hashFactory = hashFactory;
        }

        public bool MatchMasterPassword( SecureString masterPassword, PasswordDigest passwordDigest )
        {
            IPasswordGenerator generator = _generators.FirstOrDefault( g => g.Id == passwordDigest.PasswordGeneratorId );
            if ( generator == null )
                return false;
            string generatedPassword = generator.MakePassword( passwordDigest.Key, masterPassword );
            return _hashFactory.GetHash( )
                .Append( PasswordDigester.DigestSalt, Encoding.UTF8 )
                .Append( generatedPassword, Encoding.UTF8 )
                .GetValue( )
                .SequenceEqual( passwordDigest.Hash );
        }

        private readonly IHashFactory _hashFactory;
        private readonly IEnumerable<IPasswordGenerator> _generators;
    }
}