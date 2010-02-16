using System;

namespace Chwthewke.PasswordManager.Engine
{
    [ Obsolete ]
    internal class ArrayBaseConverter : BaseConverterBase
    {
        public ArrayBaseConverter( int theBase ) : base( theBase ) {}

        protected override byte[ ] ConvertBytesToDigitsCore( byte[ ] bytes, int numDigits )
        {
            int buffSize = BytesNeeded( numDigits );
            byte[ ] buff = new byte[buffSize];
            Array.Copy( bytes, buff, buffSize );

            ByteArrayDecomposable decomposable = new ByteArrayDecomposable( bytes, BytesNeeded( numDigits ) );

            byte[ ] result = new byte[numDigits];

            for ( int i = 0; i < numDigits; i++ )
            {
                result[ i ] = ( byte ) decomposable.Modulo( Base );
                decomposable.DivideBy( Base );
            }

            return result;
        }
    }
}