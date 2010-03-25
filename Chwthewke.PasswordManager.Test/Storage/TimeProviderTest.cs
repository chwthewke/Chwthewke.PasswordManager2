using System;
using Chwthewke.PasswordManager.Storage;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Storage
{
    [ TestFixture ]
    public class TimeProviderTest
    {
        [ Test ]
        public void TimeProviderReturnsSensibleDate( )
        {
            // Setup
            ITimeProvider timeProvider = new TimeProvider( );
            // Exercise
            DateTime dateTime = timeProvider.Now;
            // Verify
            Assert.That( dateTime.CompareTo( new DateTime( 2000, 1, 1 ) ), Is.EqualTo( 1 ) );
            Assert.That( dateTime.CompareTo( new DateTime( 2100, 1, 1 ) ), Is.EqualTo( -1 ) );
        }
    }
}