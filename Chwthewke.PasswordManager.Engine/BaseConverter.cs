using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Chwthewke.PasswordManager.Engine
{
    internal class BaseConverter
    {
        public int BytesNeeded( int numDigits )
        {
            return (int) Math.Ceiling( numDigits * Math.Log( _base, 256 ) );
        }

        public byte[ ] ConvertBytesToDigits( byte[ ] bytes, int numDigits )
        {
            if ( BytesNeeded( numDigits ) > bytes.Length )
                throw new ArgumentException( "Insufficient bytes provided to convert to this number of digits" );

            BigInteger value = CreatePositiveBigIntegerFromUsedBytes( numDigits, bytes );

            return EnumerateDigits( value, numDigits ).ToArray( );
        }

        public BaseConverter( int theBase ) : base( )
        {
            if ( theBase < 2 || theBase > 256 )
                throw new ArgumentException( "The base must lie between 2 and 256", "theBase" );
            _base = theBase;
        }

        private BigInteger CreatePositiveBigIntegerFromUsedBytes( int numDigits, byte[ ] bytes )
        {
            int bytesNeeded = BytesNeeded( numDigits );
            byte[ ] usedBytes = new byte[ bytesNeeded + 1 ];
            Array.Copy( bytes, usedBytes, bytesNeeded );

            return new BigInteger( usedBytes );
        }

        private IEnumerable<byte> EnumerateDigits( BigInteger value, int numDigits )
        {
            for ( int i = 0; i < numDigits; ++i )
            {
                BigInteger digit = value % _base;
                value /= _base;
                yield return (byte) digit;
            }
        }

        private readonly int _base;
    }
}