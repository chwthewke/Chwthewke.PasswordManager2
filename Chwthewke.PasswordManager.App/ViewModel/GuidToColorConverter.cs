using System;
using System.Windows.Media;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    public class GuidToColorConverter : IGuidToColorConverter
    {
        public GuidToColorConverter( double minSaturation,
                                     double maxSaturation,
                                     double minLightness,
                                     double maxLightness )
        {
            _minSaturation = minSaturation;
            _maxSaturation = maxSaturation;
            _minLightness = minLightness;
            _maxLightness = maxLightness;
        }

        public Color Convert( Guid guid )
        {
            byte[ ] bytes = guid.ToByteArray( );
            double hue = 360d * Take16BitDouble( bytes, 0 );
            double saturation = _minSaturation + Take16BitDouble( bytes, 2 ) * ( _maxSaturation - _minSaturation );
            double lightness = _minLightness + Take16BitDouble( bytes, 4 ) * ( _maxLightness - _minLightness );

            return Hsl.ToRgb( hue, saturation, lightness );
        }

        private static double Take16BitDouble( byte[ ] src, int offset )
        {
            return ( src[ offset ] * 256d + src[ offset + 1 ] ) / ( 256d * 256d );
        }

        private readonly double _minSaturation;
        private readonly double _maxSaturation;
        private readonly double _minLightness;
        private readonly double _maxLightness;
    }
}