using System;
using System.Collections.Generic;
using System.Security;
using System.Text;
using System.Linq;

namespace Chwthewke.PasswordManager.Engine
{
    [Obsolete]
    internal class PasswordGenerator : IPasswordGenerator
    {
        public const string Salt = "tsU&yUaZulAs4eOV";

        public Guid Id
        {
            get { return _id; }
        }

        public PasswordGenerator( Guid id,
                                  IHashFactory hashFactory,
                                  IBaseConverter converter,
                                  Alphabet alphabet,
                                  int length )
        {
            if ( hashFactory == null )
                throw new ArgumentNullException( "hashFactory" );
            if ( converter == null )
                throw new ArgumentNullException( "converter" );
            if ( alphabet == null )
                throw new ArgumentNullException( "alphabet" );
            if ( converter.BytesNeeded( length ) > hashFactory.HashSize )
                throw new ArgumentException( "Requested password length too large", "length" );

            _hashFactory = hashFactory;
            _converter = converter;
            _length = length;
            _alphabet = alphabet;
            _id = id;
        }

        public string MakePassword( string key, SecureString masterPassword )
        {
            return MakePasswords( key, masterPassword ).First( );
        }

        public IEnumerable<string> MakePasswords( string key, SecureString masterPassword )
        {
            string previousPassword = key;
            while ( true )
            {
                byte[ ] hash = HashTogetherWithSalt( previousPassword, masterPassword );
                yield return previousPassword = PasswordOfHash( hash );
            }
// ReSharper disable FunctionNeverReturns
        }
// ReSharper restore FunctionNeverReturns

        private string PasswordOfHash( byte[ ] hash )
        {
            return _alphabet.ToString( _converter.ConvertBytesToDigits( hash, _length ) );
        }

        private byte[ ] HashTogetherWithSalt( string key, SecureString masterPassword )
        {
            return _hashFactory.GetHash( )
                .Append( Salt, Encoding.UTF8 )
                .Append( masterPassword, Encoding.UTF8 )
                .Append( key, Encoding.UTF8 )
                .GetValue( );
        }

        private readonly IHashFactory _hashFactory;
        private readonly IBaseConverter _converter;
        private readonly Alphabet _alphabet;
        private readonly int _length;
        private readonly Guid _id;
    }
}