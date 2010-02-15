using System;

namespace Chwthewke.PasswordManager.Engine
{
    internal class ByteArrayDecomposable
    {
        private readonly byte[ ] _bytes;

        internal ByteArrayDecomposable( byte[ ] src, int srcLength )
        {
            if ( srcLength > src.Length )
                throw new ArgumentException( "Invalid srcLength." );
            _bytes = new byte[srcLength];
            Array.Copy( src, _bytes, srcLength );
        }

        internal int Modulo( int divisor )
        {
            if ( divisor < 1 )
                throw new ArgumentException( "Invalid divisor value", "divisor" );

            long runningByteValue = 1;
            int result = 0;

            for ( int i = 0 ; i < _bytes.Length ; i++ )
            {
                result = (int) ( ( result + _bytes[ i ] * runningByteValue ) % divisor );
                runningByteValue = ( runningByteValue * 256 ) % divisor;

                if ( runningByteValue == 0 )
                    break;
            }

            return result;
        }

        internal void DivideBy( int divisor )
        {
            long divided = 0;
            for ( int i = _bytes.Length - 1 ; i >= 0 ; i-- )
            {
                divided = divided * 256 + _bytes[ i ]; // 0 <= divided <= 256 * (divisor-1) + byte < 256 * divisor
                _bytes[ i ] = (byte) ( divided / divisor ); // no overflow
                divided %= divisor; // 0 <= divided < divisor
            }
        }
    }
}