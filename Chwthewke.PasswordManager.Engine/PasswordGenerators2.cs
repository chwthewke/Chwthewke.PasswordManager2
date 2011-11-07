using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    public static class PasswordGenerators2
    {
        public static readonly Guid AlphaNumeric = Guid.Parse( "{74728a10-33d4-4245-b7c9-5d72fc424c41}" );
        public static readonly Guid Full = Guid.Parse( "{ccf1451c-4b30-45a4-99b0-d54ec3c3a7ee}" );

        internal static PasswordGenerator2 GeneratorWithId( Guid guid )
        {
            return Generators[ guid ];
        }

        private static readonly IDictionary<Guid, PasswordGenerator2> Generators = GeneratorsById( );

        private static IDictionary<Guid, PasswordGenerator2> GeneratorsById( )
        {
            IDictionary<Guid, PasswordGenerator2> dictionary = new Dictionary<Guid, PasswordGenerator2>( );
            dictionary[ AlphaNumeric ] = Sha512Generator( PasswordMaterializers.AlphaNumeric );
            dictionary[ Full ] = Sha512Generator( PasswordMaterializers.Full );

            return dictionary;
        }

        private static PasswordGenerator2 Sha512Generator( PasswordMaterializer materializer )
        {
            return new PasswordGenerator2( new Sha512DerivedKeyFactory( ( s, p ) => InternalSalt.Concat( p ).Concat( s ).ToArray( ) ),
                                           new Sha512DerivedKeyFactory( ( s, p ) => s.Concat( p ).ToArray( ) ),
                                           materializer, 1, 64 );
        }

        internal static readonly byte[ ] InternalSalt = Encoding.UTF8.GetBytes( "tsU&yUaZulAs4eOV" );
    }
}