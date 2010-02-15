using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;

namespace Chwthewke.PasswordManager.Engine
{
    internal class BigIntegerBaseConverter : BaseConverterBase
    {
        public BigIntegerBaseConverter( int theBase ) : base( theBase ) {}

        protected override byte[ ] ConvertBytesToDigitsCore( byte[ ] bytes, int numDigits )
        {
            BigInteger value = CreatePositiveBigIntegerFromUsedBytes( numDigits, bytes );

            return EnumerateDigits( value, numDigits ).ToArray( );
        }

        private BigInteger CreatePositiveBigIntegerFromUsedBytes( int numDigits, byte[ ] bytes ) {
            int bytesNeeded = BytesNeeded( numDigits );
            byte[ ] usedBytes = new byte[bytesNeeded + 1];
            Array.Copy( bytes, usedBytes, bytesNeeded );

            return new BigInteger( usedBytes );
        }

        private IEnumerable<byte> EnumerateDigits( BigInteger value, int numDigits )
        {
            for ( int i = 0; i < numDigits; ++i )
            {
                Console.WriteLine( "value: {0}, base: {1}", value, Base );
                BigInteger digit = value % Base;
                value /= Base;
                Console.WriteLine( "modulo: {0}, remaining value: {1}", digit, value );
                yield return ( byte ) digit;
                Console.WriteLine( );
            }
        }
    }
}