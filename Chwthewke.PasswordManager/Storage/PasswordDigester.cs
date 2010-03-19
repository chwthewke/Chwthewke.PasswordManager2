using System;
using System.Linq;
using System.Text;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    public class PasswordDigester : IPasswordDigester
    {
        public PasswordDigester( IHash hasher, ITimeProvider timeProvider )
        {
            if ( hasher == null )
                throw new ArgumentNullException( "hasher" );
            if ( timeProvider == null )
                throw new ArgumentNullException( "timeProvider" );

            _hasher = hasher;
            _timeProvider = timeProvider;
        }

        public PasswordDigest Digest( string key,
                                      string generatedPassword,
                                      Guid masterPasswordId,
                                      Guid passwordGeneratorId,
                                      string note )
        {
            byte[ ] hash = _hasher.Hash( Encoding.UTF8.GetBytes( DigestSalt + generatedPassword ) );
            return new PasswordDigest( key, hash, masterPasswordId, passwordGeneratorId, _timeProvider.Now, note );
        }


        private readonly IHash _hasher;
        private readonly ITimeProvider _timeProvider;

        internal const string DigestSalt = @"$xZ[u(-Trf";
    }
}