using System;

namespace Chwthewke.PasswordManager.App.Services
{
    public interface ITimer
    {
        double Delay { get; }

        void Start( );
        void Stop( );

        event EventHandler Elapsed;
    }
}