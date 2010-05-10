using System;
using System.Windows.Media;
using Chwthewke.PasswordManager.App.Properties;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public static class Hsl
    {
        public static Color ToRgb( double h, double s, double l ) {
            if ( h < 0 || h >= 360 )
                throw new ArgumentOutOfRangeException( "h", h, Resources.Hsl_ToColor_h_range );
            if ( s < 0 || s > 1 )
                throw new ArgumentOutOfRangeException( "s", s, Resources.Hsl_ToColor_s_range );
            if ( l < 0 || l > 1 )
                throw new ArgumentOutOfRangeException( "l", l, Resources.Hsl_ToColor_l_range );

            double q = l < 0.5 ? l * ( 1 + s ) : ( l + s - l * s );
            double p = 2 * l - q;
            double hNorm = h / 360;

            return Color.FromRgb( ComputeComponent( p, q, hNorm + 1 / 3d ),
                                  ComputeComponent( p, q, hNorm ),
                                  ComputeComponent( p, q, hNorm - 1 / 3d ) );
        }

        private static byte ComputeComponent( double p, double q, double t )
        {
            double tNorm = ( t + 1 ) % 1;
            if ( tNorm < 1 / 6d )
                return ToByte( p + 6 * tNorm * ( q - p ) );
            if ( tNorm < 1 / 2d )
                return ToByte( q );
            if ( tNorm < 2 / 3d )
                return ToByte( p + 6 * ( 2 / 3d - tNorm ) * ( q - p ) );
            return ToByte( p );
        }

        private static byte ToByte( double d )
        {
            if ( d <= 0 )
                return 0x00;
            if ( d >= 1 )
                return 0xFF;
            return ( byte ) ( 256 * d );
        }
    }
}