using System;
using System.Text;
using Chwthewke.PasswordManager.Engine;
using NUnit.Framework;

namespace Chwthewke.PasswordManager.Test.Engine
{
    [ TestFixture ]
    [ Ignore ] // TODO figure how to setup an integration category
    public class Pbkdf2DerivedKeyFactoryTest
    {
        [ Test ]
        public void TimingTest( )
        {
            // Set up
            var derivedKeyFactory = new Pbkdf2DerivedKeyFactory( 1000 );
            int result = 0;
            // Exercise
            for ( int i = 0; i < 100; ++i )
            {
                var derived = derivedKeyFactory.DeriveKey( Encoding.UTF8.GetBytes( "abcd" + i ),
                                                           Encoding.UTF8.GetBytes( "1234" ), 1, 10 );
                result += derived.Length;
            }
            // Verify
            Console.WriteLine( @"result: {0}", result );
        } 
    }
}