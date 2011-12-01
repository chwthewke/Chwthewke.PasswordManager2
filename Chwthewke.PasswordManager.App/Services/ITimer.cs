using System.Timers;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface ITimer
    {
        void Start( );
        void Stop( );

        event ElapsedEventHandler Elapsed;
    }
}