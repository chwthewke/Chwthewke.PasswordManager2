using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Editor;
using Chwthewke.PasswordManager.Engine;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Editor
{
    public class IsPasswordModel : IDerivedPasswordModel
    {
        private static readonly IPasswordDerivationEngine Engine = new PasswordDerivationEngine( PasswordGenerators.Generators );

        public static IDerivedPasswordModel For( Guid generator, string key, SecureString masterPassword, int iteration )
        {
            return new IsPasswordModel( generator, Engine.Derive(
                new PasswordRequest( key, masterPassword, iteration, generator ) ) );
        }

        public static IDerivedPasswordModel Empty( Guid generator )
        {
            return new IsPasswordModel( generator, NullDerivedPassword.Instance );
        }

        private IsPasswordModel( Guid generator, IDerivedPassword derivedPassword )
        {
            Generator = generator;
            DerivedPassword = derivedPassword;
        }

        public Guid Generator { get; private set; }

        public IDerivedPassword DerivedPassword { get; private set; }

        public bool IsLegacy
        {
            get { return Engine.LegacyPasswordGeneratorIds.Contains( Generator ); }
        }
    }

    public class DerivedPasswordEqualityComparer : IEqualityComparer<IDerivedPasswordModel>
    {
        public bool Equals( IDerivedPasswordModel x, IDerivedPasswordModel y )
        {
            if ( ReferenceEquals( x, y ) )
                return true;

            if ( ReferenceEquals( null, y ) || ReferenceEquals( null, x ) )
                return false;

            return y.Generator.Equals( x.Generator ) && Equals( y.DerivedPassword, x.DerivedPassword );
        }

        public int GetHashCode( IDerivedPasswordModel o )
        {
            unchecked
            {
                int result = o.Generator.GetHashCode( );
                result = ( result * 397 ) ^ o.DerivedPassword.GetHashCode( );
                return result;
            }
        }
    }
}