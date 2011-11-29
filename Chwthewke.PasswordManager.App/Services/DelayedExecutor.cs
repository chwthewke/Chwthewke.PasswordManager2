using System;
using System.Collections.Generic;
using System.Timers;

namespace Chwthewke.PasswordManager.App.Services
{
    internal class ExclusiveExecutor
    {
        private volatile int _sequenceNumber;
        private readonly object _lockObject = new object( );

        public int Next
        {
            get
            {
                lock ( _lockObject )
                {
                    _sequenceNumber += 1;
                    return _sequenceNumber;
                }
            }
        }

        public void ExecuteIfCurrent( int number, Action action )
        {
            if ( number == _sequenceNumber )
                action( );
        }
    }

    internal class DelayedScheduler
    {
        private readonly ICollection<Timer> _timers = new HashSet<Timer>( );
        private readonly object _lockObject = new object(  );

        public void Schedule( Action action, double interval )
        {
            StartTimer( (s,e) => action(), interval );
        }

        private void StartTimer( ElapsedEventHandler handler, double timerDuration )
        {
            var timer = new Timer( timerDuration );
            timer.Elapsed += RemoveTimer;
            timer.Elapsed += handler;

            lock ( _lockObject )
            {
                _timers.Add( timer );
                timer.Start( );
            }
        }

        private void RemoveTimer( object source, ElapsedEventArgs args )
        {
            Timer timer = source as Timer;
            if ( timer == null )
                return;

            lock ( _lockObject )
            {
                timer.Stop( );
                _timers.Remove( timer );
            }
        }

    }

    internal class DelayedExecutor
    {
        public const int TimerDuration = 100;

        public DelayedExecutor( int timerDuration = TimerDuration )
        {
            _timerDuration = timerDuration;
            _activeTimers = new HashSet<Timer>( );
        }

        public void Execute( Action<Func<bool>> cancelableAction )
        {
            _sequenceNumber++;
            var task = new CancellableActionTask( _sequenceNumber, this, cancelableAction );
            StartTimer( ( s, e ) => task.Execute( ) );
        }

        internal int SequenceNumber
        {
            get { return _sequenceNumber; }
        }

        private void StartTimer( ElapsedEventHandler handler )
        {
            var timer = new Timer( _timerDuration );
            timer.Elapsed += RemoveTimer;
            timer.Elapsed += handler;

            lock ( _activeTimers )
            {
                _activeTimers.Add( timer );
                timer.Start( );
            }
        }

        private void RemoveTimer( object source, ElapsedEventArgs args )
        {
            Timer timer = source as Timer;
            if ( timer == null )
                return;

            lock ( _activeTimers )
            {
                timer.Stop( );
                _activeTimers.Remove( timer );
            }
        }


        private volatile int _sequenceNumber;
        private readonly int _timerDuration;

        private readonly ICollection<Timer> _activeTimers;

    }

    class CancellableActionTask
    {
        public CancellableActionTask( int sequenceNumber, DelayedExecutor executor, Action<Func<bool>> action )
        {
            _sequenceNumber = sequenceNumber;
            _executor = executor;
            _action = action;
        }

        private readonly int _sequenceNumber;
        private readonly DelayedExecutor _executor;
        private readonly Action<Func<bool>> _action;

        public void Execute()
        {
            _action.Invoke( ( ) => _sequenceNumber != _executor.SequenceNumber );
        }
    }

}