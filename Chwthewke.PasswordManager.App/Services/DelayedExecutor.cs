using System;

namespace Chwthewke.PasswordManager.App.Services
{
    public class DelayedExecutor
    {
        private volatile int _sequenceNumber;

        public void Execute( Action<Func<bool>> cancelableAction )
        {
            _sequenceNumber++;
        }
    }

}