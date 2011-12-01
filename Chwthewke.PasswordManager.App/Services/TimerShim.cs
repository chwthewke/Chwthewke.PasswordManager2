using System.Timers;

namespace Chwthewke.PasswordManager.App.Services
{
    public class TimerShim : ITimer
    {

        public TimerShim( double delay )
        {
            _timer = new Timer( delay );
        }

        public void Start( )
        {
            _timer.Start( );
        }

        public void Stop( )
        {
            _timer.Stop( );
        }

        public event ElapsedEventHandler Elapsed
        {
            add { _timer.Elapsed += value; }
            remove { _timer.Elapsed -= value; }
        }

        private readonly Timer _timer;
    }
}