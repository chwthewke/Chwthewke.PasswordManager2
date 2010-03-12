using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Editor
{
    public class PasswordEditor : IPasswordEditor
    {
        public PasswordEditor( IEnumerable<IPasswordGenerator> generators )
        {
            Key = string.Empty;
            _passwordGenerators = new List<IPasswordGenerator>( generators );
        }

        public string Key { get; set; }

        public void Reset( )
        {
            Key = string.Empty;
            _generatedPasswords.Clear( );
        }

        public void GeneratePasswords( SecureString masterPassword )
        {
            if ( string.IsNullOrWhiteSpace( Key ) )
                throw new InvalidOperationException( "Cannot generate passwords with an empty key." );
            foreach ( IPasswordGenerator generator in PasswordSlots )
            {
                _generatedPasswords[ generator ] = generator.MakePassword( Key, masterPassword );
            }
        }

        public IEnumerable<IPasswordGenerator> PasswordSlots
        {
            get { return _passwordGenerators; }
        }

        public IPasswordGenerator SavedSlot
        {
            get { return null; }
        }

        public IPasswordDocument GeneratedPassword( IPasswordGenerator slot )
        {
            string generatedPassword;
            _generatedPasswords.TryGetValue( slot, out generatedPassword );

            return generatedPassword == null ? null : new PasswordDocument( generatedPassword );
        }

        private readonly IDictionary<IPasswordGenerator, string> _generatedPasswords =
            new Dictionary<IPasswordGenerator, string>( );

        private readonly IList<IPasswordGenerator> _passwordGenerators;
    }
}