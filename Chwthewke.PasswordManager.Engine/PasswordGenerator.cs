using System.Text;

namespace Chwthewke.PasswordManager.Engine
{
    internal class PasswordGenerator
    {
        private readonly IDerivedKeyFactory _derivedPasswordFactory;
        private readonly IDerivedKeyFactory _digestFactory;
        private readonly PasswordMaterializer _materializer;

        private readonly int _digestLength;

        internal static readonly byte[ ] DigestSalt = new byte[ ]
                                                          {
                                                              0x24, 0x78, 0x5a, 0x5b, 0x75,
                                                              0x28, 0x2d, 0x54, 0x72, 0x66,
                                                          };

        public PasswordGenerator( IDerivedKeyFactory derivedPasswordFactory, IDerivedKeyFactory digestFactory,
                                  PasswordMaterializer materializer, int digestLength )
        {
            _derivedPasswordFactory = derivedPasswordFactory;
            _digestFactory = digestFactory;
            _materializer = materializer;
            _digestLength = digestLength;
        }

        public DerivedPassword Derive( PasswordRequest request )
        {
            return request.MasterPassword.ConsumeBytes( Encoding.UTF8, bytes => DeriveInternal( request, bytes ) );
        }

        private DerivedPassword DeriveInternal( PasswordRequest request, byte[ ] passwordBytes )
        {
            byte[ ] derivedPasswordBytes =
                _derivedPasswordFactory.DeriveKey( GetBytes( request.Key ), passwordBytes, request.Iterations, _materializer.BytesNeeded );

            string derivedPassword =
                _materializer.ToString( derivedPasswordBytes );

            byte[ ] digestBytes =
                _digestFactory.DeriveKey( DigestSalt, GetBytes( derivedPassword ), 1, _digestLength );

            return new DerivedPassword( derivedPassword,
                                        new PasswordDigest( request.Key, digestBytes, request.Iterations, request.PasswordGenerator ) );
        }


        private byte[ ] GetBytes( string str )
        {
            return Encoding.UTF8.GetBytes( str );
        }
    }
}