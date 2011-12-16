using System;
using System.Diagnostics;
using System.Threading;
using Chwthewke.PasswordManager.App.Services;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.App.Services
{
    [ TestFixture ]
    [ Category( "SystemTimer" ) ]
    public class TimerShimTest
    {
        private TimerShim _shim;
        private Stopwatch _watch;

        [SetUp]
        public void SetupWatch( )
        {
            _watch = new Stopwatch( );
        }

        [ TearDown ]
        public void StopTimerShim( )
        {
            if (_shim != null)
                _shim.Stop( );
            _watch.Reset( );
        }

        [ Test ]
        public void TimerShimStartsTimer( )
        {
            // Set up
            _shim = new TimerShim( 10 );
            // Exercise
            _shim.Start( );
            // Verify
            Assert.That( _shim.Timer.Enabled, Is.True );
            Assert.That( _shim.Delay, Is.EqualTo( 10d ).Within( 0.000001d ) );
        }

        [ Test ]
        public void TimerShimCompletesWithinDelay( )
        {
            // Set up
            _shim = new TimerShim( 50 );
            _shim.Elapsed += ( s, e ) => _watch.Stop( );
            _watch.Start( );
            // Exercise
            _shim.Start( );
            // Verify
            Thread.Sleep( TimeSpan.FromMilliseconds( 100 ) );
            Assert.That( _watch.IsRunning, Is.False );
            Assert.That( _watch.ElapsedMilliseconds, Is.EqualTo( 50 ).Within( 20 ) );
        }

        [ Test ]
        public void TimerShimPassesSelfInEvent( )
        {
            // Set up
            ITimer timer = null;
            _shim = new TimerShim( 50 );
            _shim.Elapsed += ( s, e ) => { timer = s as ITimer; };

            // Exercise
            _shim.Start( );
            // Verify
            Thread.Sleep( TimeSpan.FromMilliseconds( 100 ) );
            Assert.That( timer, Is.SameAs( _shim ) );
        }

        [Test]
        public void StoppedTimerShimDoesNotRaiseElapsed( )
        {
            // Set up
            _shim = new TimerShim( 50 );
            _shim.Elapsed += ( s, e ) => _watch.Stop( );
            _watch.Start( );

            _shim.Start( );
            // Exercise
            _shim.Stop( );

            // Verify
            Thread.Sleep( TimeSpan.FromMilliseconds( 100 ) );
            Assert.That( _watch.IsRunning, Is.True );
        }
    
        [Test]
        public void StoppedTimerShimDisablesTimer( )
        {
            // Set up
            _shim = new TimerShim( 50 );

            _shim.Start( );
            // Exercise
            _shim.Stop( );

            // Verify
            Assert.That( _shim.Timer.Enabled, Is.False );
        }
    }
}