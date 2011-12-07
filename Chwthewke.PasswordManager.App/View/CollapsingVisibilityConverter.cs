using System;
using System.Globalization;
using System.Windows;
using System.Windows.Data;

namespace Chwthewke.PasswordManager.App.View
{
    internal class CollapsingVisibilityConverter : IValueConverter
    {
        public object Convert( object value, Type targetType, object parameter, CultureInfo culture )
        {
            if ( value is bool )
                return ( (bool) value ) ? Visibility.Visible : Visibility.Collapsed;
            throw new ArgumentException( "'value' must be of type bool." );
        }

        public object ConvertBack( object value, Type targetType, object parameter, CultureInfo culture )
        {
            throw new NotImplementedException( );
        }
    }
}