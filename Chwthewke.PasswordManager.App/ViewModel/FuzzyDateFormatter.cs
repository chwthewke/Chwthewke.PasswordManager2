using System;
using System.Globalization;
using Chwthewke.PasswordManager.Editor;

namespace Chwthewke.PasswordManager.App.ViewModel
{
    internal class FuzzyDateFormatter : IFuzzyDateFormatter
    {
        public FuzzyDateFormatter( ITimeProvider timeProvider )
        {
            _timeProvider = timeProvider;
        }

        public string Format( DateTime dateTime )
        {
            if ( IsToday( dateTime ) )
                return string.Format( "Today {0}", dateTime.ToString( "t", DefaultCulture ) );
            if ( IsThisYear( dateTime ) )
                return string.Format( "{0} {1}", dateTime.ToString( "m", DefaultCulture ), dateTime.ToString( "t", DefaultCulture ) );
            return dateTime.ToString( "f", DefaultCulture );
        }

        private bool IsToday( DateTime dateTime )
        {
            return dateTime.Date == _timeProvider.Now.Date;
        }

        private bool IsThisYear( DateTime dateTime )
        {
            return dateTime.Year == _timeProvider.Now.Year;
        }

        private readonly ITimeProvider _timeProvider;
        private static readonly CultureInfo DefaultCulture = new CultureInfo( "en-US" );
    }
}