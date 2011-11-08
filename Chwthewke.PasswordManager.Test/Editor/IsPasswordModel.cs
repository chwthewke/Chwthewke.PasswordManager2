using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Test.Editor
{
    public class IsPasswordModel : IDerivedPasswordModel
    {
        private static readonly IPasswordDerivationEngine Engine = new PasswordDerivationEngine( PasswordGenerators2.Generators );

        public static IDerivedPasswordModel For( Guid generator, string key, SecureString masterPassword, int iteration )
        {
            return new IsPasswordModel( generator, iteration, Engine.Derive(
                new PasswordRequest( key, masterPassword, iteration, generator ) ) );
        }

        public static IDerivedPasswordModel Empty( Guid generator, int iteration )
        {
            return new IsPasswordModel( generator, iteration, null );
        }

        private IsPasswordModel( Guid generator, int iteration, DerivedPassword derivedPassword )
        {
            Generator = generator;
            DerivedPassword = derivedPassword;
            Iteration = iteration;
        }

        public Guid Generator { get; private set; }

        public DerivedPassword DerivedPassword { get; private set; }

        public int Iteration { get; set; }
    }

    public class DerivedPasswordEqualityComparer : IEqualityComparer<IDerivedPasswordModel>
    {
        public bool Equals( IDerivedPasswordModel x, IDerivedPasswordModel y )
        {
            if ( ReferenceEquals( x, y ) )
                return true;

            if ( ReferenceEquals( null, y ) || ReferenceEquals( null, x ) )
                return false;

            return y.Generator.Equals( x.Generator ) && Equals( y.DerivedPassword, x.DerivedPassword ) && y.Iteration == x.Iteration;
        }

        public int GetHashCode( IDerivedPasswordModel o )
        {
            unchecked
            {
                int result = o.Generator.GetHashCode( );
                result = ( result * 397 ) ^ o.DerivedPassword.GetHashCode( );
                result = ( result * 397 ) ^ o.Iteration;
                return result;
            }
        }
    }
}