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
        public MasterPasswordMatcher( IEnumerable<IPasswordGenerator> generators, IHash hash )
        {
            _generators = new List<IPasswordGenerator>( generators );
            _hash = hash;
        }

        public bool MatchMasterPassword( SecureString masterPassword, PasswordDigest passwordDigest )
        {
            IPasswordGenerator generator = _generators.FirstOrDefault( g => g.Id == passwordDigest.PasswordGeneratorId );
            if ( generator == null )
                return false;
            string generatedPassword = generator.MakePassword( passwordDigest.Key, masterPassword );
            return
                _hash.Hash( Encoding.UTF8.GetBytes( PasswordDigester.DigestSalt + generatedPassword ) ).SequenceEqual(
                    passwordDigest.Hash );
        }

        private readonly IHash _hash;
        private readonly IEnumerable<IPasswordGenerator> _generators;
    }
}