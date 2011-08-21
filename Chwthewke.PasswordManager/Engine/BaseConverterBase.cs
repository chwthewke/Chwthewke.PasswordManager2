using System;

namespace Chwthewke.PasswordManager.Engine
{
    internal abstract class BaseConverterBase : IBaseConverter
    {
        private readonly int _base;

        protected BaseConverterBase( int theBase )
        {
            if ( theBase < 2 || theBase > 256 )
                throw new ArgumentException( "The base must lie between 2 and 256", "theBase" );
            _base = theBase;
        }

        public int Base
        {
            get { return _base; }
        }

        public int BytesNeeded( int numDigits )
        {
            return (int) Math.Ceiling( numDigits * Math.Log( _base, 256 ) );
        }

        public byte[ ] ConvertBytesToDigits( byte[ ] bytes, int numDigits )
        {
            if ( BytesNeeded( numDigits ) > bytes.Length )
                throw new ArgumentException( "Insufficient bytes provided to convert to this number of digits" );
            return ConvertBytesToDigitsCore( bytes, numDigits );
        }

        protected abstract byte[ ] ConvertBytesToDigitsCore( byte[ ] bytes, int numDigits );
    }
}