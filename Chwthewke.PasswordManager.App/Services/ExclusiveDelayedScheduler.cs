using System;
using System.Collections.Generic;
using System.Timers;

namespace Chwthewke.PasswordManager.App.Services
{
    public class ExclusiveDelayedScheduler : IExclusiveDelayedScheduler
    {
        public ExclusiveDelayedScheduler( Func<double, ITimer> timerFactory )
        {
            _timerFactory = timerFactory;
        }

        public void ScheduleActions( double delay, IEnumerable<Action> actions )
        {
            int actionNumber;
            lock ( _lockObject )
            {
                actionNumber = ++_sequenceNumber;
            }

            Action compoundAction =
                ( ) =>
                    {
                        foreach ( var action in actions )
                        {
                            lock ( _lockObject )
                            {
                                if ( _sequenceNumber != actionNumber )
                                    return;
                            }
                            action( );
                        }
                    };
            StartTimer( delay, compoundAction );
        }

        private void StartTimer( double delay, Action action )
        {
            var timer = _timerFactory( delay );

            timer.Elapsed += RemoveTimer;
            timer.Elapsed += ( s, e ) => action( );

            lock ( _lockObject )
            {
                _timers.Add( timer );
                timer.Start( );
            }
        }


        private void RemoveTimer( object source, EventArgs e )
        {
            var timer = source as ITimer;
            if ( timer == null )
                return;

            lock ( _lockObject )
            {
                timer.Stop( );
                _timers.Remove( timer );
            }
        }

        private volatile int _sequenceNumber = 0;
        private readonly object _lockObject = new object( );
        private readonly ICollection<ITimer> _timers = new HashSet<ITimer>( );
        private readonly Func<double, ITimer> _timerFactory;

    }
}