using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal static class PasswordGenerators2
    {
        internal static readonly Guid AlphaNumeric = Guid.Parse( "{74728a10-33d4-4245-b7c9-5d72fc424c41}" );
        internal static readonly Guid Full = Guid.Parse( "{ccf1451c-4b30-45a4-99b0-d54ec3c3a7ee}" );

        internal static readonly IDictionary<Guid, PasswordGenerator2> Generators =
            new[ ]
                {
                    new { Id = AlphaNumeric, Generator = Sha512Generator( PasswordMaterializers.AlphaNumeric ) },
                    new { Id = Full, Generator = Sha512Generator( PasswordMaterializers.Full ) },
                }
                .ToDictionary( z => z.Id, z => z.Generator );

        private static PasswordGenerator2 Sha512Generator( PasswordMaterializer materializer )
        {
            return new PasswordGenerator2( new Sha512DerivedKeyFactory( ( s, p ) => InternalSalt.Concat( p ).Concat( s ).ToArray( ) ),
                                           new Sha512DerivedKeyFactory( ( s, p ) => s.Concat( p ).ToArray( ) ),
                                           materializer, 1, 64 );
        }

        internal static readonly byte[ ] InternalSalt = Encoding.UTF8.GetBytes( "tsU&yUaZulAs4eOV" );
    }
}