using System;
using System.ComponentModel;

namespace Chwthewke.PasswordManager.App.Services
{
    public class ActionBackgroundWorker : BackgroundWorker
    {
        protected override void OnRunWorkerCompleted( RunWorkerCompletedEventArgs e )
        {
            var job = e.UserState as ActionJob;
            if ( job == null )
                return;
            job.WorkComplete( );
        }

        protected override void OnDoWork( DoWorkEventArgs e )
        {
            var job = e.Argument as ActionJob;
            if ( job == null ) 
                return;
            job.Work( );
        }
    }

    public class ActionJob
    {
        public ActionJob( Action work, Action workComplete )
        {
            _work = work;
            _workComplete = workComplete;
        }

        public Action Work
        {
            get { return _work; }
        }

        public Action WorkComplete
        {
            get { return _workComplete; }
        }

        private Action _work;
        private Action _workComplete;
    }
}