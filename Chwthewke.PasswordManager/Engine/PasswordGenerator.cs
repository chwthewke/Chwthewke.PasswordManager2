using System;
using System.Security;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
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
            if ( converter.Base != alphabet.Length )
                throw new ArgumentException( "The converter's base must match the alphabet length" );
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
            byte[ ] hash = HashTogetherWithSalt( key, masterPassword );
            byte[ ] passwordBytes = _converter.ConvertBytesToDigits( hash, _length );
            return _alphabet.ToString( passwordBytes );
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