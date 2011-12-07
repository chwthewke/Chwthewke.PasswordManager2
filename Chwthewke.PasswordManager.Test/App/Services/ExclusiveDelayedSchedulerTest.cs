using System;
using System.Collections.Generic;
using Autofac;
using Chwthewke.PasswordManager.App.Services;
using Moq;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.Services
{
    [ TestFixture ]
    public class ExclusiveDelayedSchedulerTest
    {
        // ReSharper disable MemberCanBePrivate.Global
        // ReSharper disable UnusedAutoPropertyAccessor.Global
        public IExclusiveDelayedScheduler Scheduler { get; set; }
        public IList<Mock<ITimer>> TimerMocks { get; set; }
        // ReSharper restore UnusedAutoPropertyAccessor.Global
        // ReSharper restore MemberCanBePrivate.Global

        [ SetUp ]
        public void SetUpMockTimers( )
        {
            TestInjection.TestContainer( new ProvideTimerMocksModule( ) ).InjectProperties( this );
        }

        [ Test ]
        public void ScheduleActionCreatesTimerWithDelayAndStartsIt( )
        {
            // Set up

            // Exercise
            Scheduler.ScheduleActions( 113, new Action[ ] { } );
            // Verify
            Assert.That( TimerMocks, Has.Count.EqualTo( 1 ) );
            Assert.That( TimerMocks[ 0 ].Object.Delay, Is.EqualTo( 113 ) );
            TimerMocks[ 0 ].Verify( t => t.Start( ) );
        }

        [ Test ]
        public void WhenTimerElapsedActionIsExecutedAndTimerIsStopped( )
        {
            // Set up
            bool executed = false;
            Scheduler.ScheduleActions( 100, new Action[ ] { ( ) => executed = true } );
            var timerMock = TimerMocks[ 0 ];
            // Exercise
            timerMock.Raise( t => t.Elapsed += null, EventArgs.Empty );

            // Verify
            Assert.That( executed, Is.True );
            timerMock.Verify( t => t.Stop( ) );
        }

        [ Test ]
        public void WhenAnotherActionIsScheduledBeforeTimerElapsedTimerIsStoppedButFirstActionIsNotExecuted( )
        {
            // Set up
            bool executed = false;
            Scheduler.ScheduleActions( 100, new Action[ ] { ( ) => executed = true } );
            var timerMock = TimerMocks[ 0 ];

            Scheduler.ScheduleActions( 120, new Action[ ] { } );
            // Exercise
            timerMock.Raise( t => t.Elapsed += null, EventArgs.Empty );

            // Verify
            Assert.That( executed, Is.False );
            timerMock.Verify( t => t.Stop( ) );
        }

        [ Test ]
        public void WhenAnotherActionIsScheduledWhileFirstActionExecutesFurtherPartsAreSkipped( )
        {
            // Set up
            bool executed = false;
            Scheduler.ScheduleActions( 100, new Action[ ]
                                                {
                                                    ( ) => Scheduler.ScheduleActions( 110, new Action[ ] { } ),
                                                    ( ) => executed = true
                                                } );
            var timerMock = TimerMocks[ 0 ];

            // Exercise
            timerMock.Raise( t => t.Elapsed += null, EventArgs.Empty );

            // Verify
            Assert.That( executed, Is.False );
            timerMock.Verify( t => t.Stop( ) );
        }
    }

    internal class ProvideTimerMocksModule : Module
    {
        private List<Mock<ITimer>> _timerMocks;

        protected override void Load( ContainerBuilder builder )
        {
            _timerMocks = new List<Mock<ITimer>>( );

            builder.RegisterInstance( _timerMocks ).As<IList<Mock<ITimer>>>( );

            builder.RegisterInstance<Func<double, ITimer>>( GetTimerMock ).As<Func<double, ITimer>>( );
        }

        private ITimer GetTimerMock( double delay )
        {
            var timerMock = new Mock<ITimer>( );
            timerMock.Setup( t => t.Delay ).Returns( delay );
            _timerMocks.Add( timerMock );
            return timerMock.Object;
        }
    }
}