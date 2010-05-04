using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    internal class PasswordDigester : IPasswordDigester
    {
        public PasswordDigester( IHashFactory hashFactory, ITimeProvider timeProvider )
        {
            if ( hashFactory == null )
                throw new ArgumentNullException( "hashFactory" );
            if ( timeProvider == null )
                throw new ArgumentNullException( "timeProvider" );

            _hashFactory = hashFactory;
            _timeProvider = timeProvider;
        }

        public PasswordDigest Digest( string key,
                                      string generatedPassword,
                                      Guid masterPasswordId,
                                      Guid passwordGeneratorId,
                                      string note )
        {
            byte[ ] hash = _hashFactory.GetHash( )
                .Append( DigestSalt, Encoding.UTF8 )
                .Append( generatedPassword, Encoding.UTF8 )
                .GetValue( );
            return new PasswordDigest( key, hash, masterPasswordId, passwordGeneratorId, _timeProvider.Now, note );
        }


        private readonly IHashFactory _hashFactory;
        private readonly ITimeProvider _timeProvider;

        internal const string DigestSalt = @"$xZ[u(-Trf";
    }
}