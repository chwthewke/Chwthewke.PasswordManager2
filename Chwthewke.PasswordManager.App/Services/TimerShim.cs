using System;
using System.Collections.Generic;
using System.Timers;

namespace Chwthewke.PasswordManager.App.Services
{
    public class TimerShim : ITimer
    {
        public TimerShim( double delay )
        {
            _timer = new Timer( delay );
        }

        public double Delay
        {
            get { return _timer.Interval; }
        }

        public void Start( )
        {
            _timer.Start( );
        }

        public void Stop( )
        {
            _timer.Stop( );
        }

        public event EventHandler Elapsed
        {
            add
            {
                ElapsedEventHandler elapsed = ( s, e ) => value( s, e );
                _handlerTranslations[ value ] = elapsed;
                _timer.Elapsed += elapsed;
            }

            remove
            {
                ElapsedEventHandler elapsed;
                _handlerTranslations.TryGetValue( value, out elapsed );
                _timer.Elapsed -= elapsed;
            }
        }

        private readonly IDictionary<EventHandler, ElapsedEventHandler> _handlerTranslations =
            new Dictionary<EventHandler, ElapsedEventHandler>( );

        private readonly Timer _timer;
    }
}