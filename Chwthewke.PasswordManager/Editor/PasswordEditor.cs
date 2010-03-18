using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    public class PasswordEditor : IPasswordEditor
    {
        public PasswordEditor( IEnumerable<IPasswordGenerator> generators,
                               IMasterPasswordFinder masterPasswordFinder,
                               IPasswordDigester digester )
        {
            _passwordGenerators = new List<IPasswordGenerator>( generators );
            _masterPasswordFinder = masterPasswordFinder;
            _digester = digester;
            Reset( );
        }


        public string Key
        {
            get { return _key; }
            set
            {
                if ( value == _key )
                    return;
                _generatedPasswordDocuments.Clear( );
                _key = value;
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                if ( _note == value )
                    return;
                _note = value;
                foreach ( var generatedPasswordDocument in _generatedPasswordDocuments.Values )
                    generatedPasswordDocument.SavablePasswordDigest.Note = _note;
            }
        }

        public void Reset( )
        {
            Key = string.Empty;
            Note = string.Empty;
        }

        public void GeneratePasswords( SecureString masterPassword )
        {
            if ( string.IsNullOrWhiteSpace( Key ) )
                throw new InvalidOperationException( "Cannot generate passwords with an empty key." );

            Guid masterPasswordId = _masterPasswordFinder.IdentifyMasterPassword( masterPassword ) ?? Guid.NewGuid( );

            foreach ( IPasswordGenerator generator in PasswordSlots )
            {
                string generatedPassword = generator.MakePassword( Key, masterPassword );
                _generatedPasswordDocuments[ generator ] =
                    new PasswordDocument( generatedPassword,
                                          _digester.Digest( Key, generatedPassword, masterPasswordId, generator.Id, Note ) );
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

        public PasswordDocument GeneratedPassword( IPasswordGenerator slot )
        {
            PasswordDocument generatedPasswordDocument;
            _generatedPasswordDocuments.TryGetValue( slot, out generatedPasswordDocument );

            return generatedPasswordDocument;
        }

        private string _key;
        private string _note;

        private readonly IDictionary<IPasswordGenerator, PasswordDocument> _generatedPasswordDocuments =
            new Dictionary<IPasswordGenerator, PasswordDocument>( );

        private readonly IList<IPasswordGenerator> _passwordGenerators;
        private readonly IMasterPasswordFinder _masterPasswordFinder;
        private readonly IPasswordDigester _digester;
    }
}