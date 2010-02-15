using System;

namespace Chwthewke.PasswordManager.Engine
{
    internal class ArrayBaseConverter : IBaseConverter
    {
        private readonly int _base;

        public ArrayBaseConverter( int @base )
        {
            if ( @base < 2 || @base > 256 )
                throw new ArgumentException( "The base must lie between 2 and 256", "base" );
            _base = @base;
        }

        public int Base { get { return _base; } }

        public int UsedBytes( int length )
        {
            return (int) Math.Ceiling( length * Math.Log( _base, 256 ) );
        }

        public byte[ ] Convert( byte[ ] src, int length )
        {
            int buffSize = UsedBytes( length );
            byte[ ] buff = new byte[buffSize];
            Array.Copy( src, buff, buffSize );

            ByteArrayDecomposable decomposable = new ByteArrayDecomposable( src, UsedBytes( length ) );

            byte[ ] result = new byte[length];

            for ( int i = 0 ; i < length ; i++ )
            {
                result[ i ] = (byte) decomposable.Modulo( _base );
                decomposable.DivideBy( _base );
            }

            return result;
        }
    }
}