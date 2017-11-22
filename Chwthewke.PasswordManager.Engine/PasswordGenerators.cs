using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal static class PasswordGenerators
    {
        internal static readonly Guid LegacyAlphaNumeric = Guid.Parse( "{74728a10-33d4-4245-b7c9-5d72fc424c41}" );
        internal static readonly Guid LegacyFull = Guid.Parse( "{ccf1451c-4b30-45a4-99b0-d54ec3c3a7ee}" );

        internal static readonly Guid AlphaNumeric = Guid.Parse("{3b120ddb-a21b-40ac-b028-e086ed8b23c1}");
        internal static readonly Guid Full = Guid.Parse("{09ec0117-0661-4bef-8248-ef8e3ea0eb81}");

        internal static readonly Guid AlphaNumeric2017 = Guid.Parse("{feb68a3b-6856-40eb-b527-25684d81a35a}");
        internal static readonly Guid Full2017 = Guid.Parse("{cbc926e5-75f2-4a3c-9981-c7dce200aed6}");


        internal static readonly IDictionary<Guid, PasswordGenerator> Generators =
            new[ ]
                {
                    new { Id = LegacyAlphaNumeric, Generator = Sha512Generator( PasswordMaterializers.AlphaNumeric ) },
                    new { Id = LegacyFull, Generator = Sha512Generator( PasswordMaterializers.Full ) },
                    new { Id = AlphaNumeric, Generator = Pbkdf2Generator( PasswordMaterializers.AlphaNumeric ) },
                    new { Id = Full, Generator = Pbkdf2Generator( PasswordMaterializers.Full ) },
                    new { Id = AlphaNumeric2017, Generator = Pbkdf2Generator( PasswordMaterializers.AlphaNumeric2017 ) },
                    new { Id = Full2017, Generator = Pbkdf2Generator( PasswordMaterializers.Full2017 ) },
                }
                .ToDictionary( z => z.Id, z => z.Generator );

        private static PasswordGenerator Pbkdf2Generator( PasswordMaterializer materializer )
        {
            return new PasswordGenerator( new Pbkdf2DerivedKeyFactory( 1000 ), new Pbkdf2DerivedKeyFactory( 1000 ), materializer, 32 );
        }

        private static PasswordGenerator Sha512Generator( PasswordMaterializer materializer )
        {
            return new PasswordGenerator( new Sha512DerivedKeyFactory( ( s, p ) => InternalSalt.Concat( p ).Concat( s ).ToArray( ) ),
                                          new Sha512DerivedKeyFactory( ( s, p ) => s.Concat( p ).ToArray( ) ),
                                          materializer, 64 );
        }

        internal static readonly byte[ ] InternalSalt = Encoding.UTF8.GetBytes( "tsU&yUaZulAs4eOV" );
    }
}