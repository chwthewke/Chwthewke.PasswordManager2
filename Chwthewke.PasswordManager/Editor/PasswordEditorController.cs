using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorController : IPasswordEditorController
    {
        public PasswordEditorController( IPasswordStore passwordStore, IPasswordDigester digester, 
            Func<Guid> newGuidFactory, IEnumerable<IPasswordGenerator> generators )
        {
            _passwordStore = passwordStore;
            _newGuidFactory = newGuidFactory;
            _digester = digester;
            _key = string.Empty;
            Note = string.Empty;
            MasterPassword = new SecureString( );
            Generators = generators;
        }

        public string Key
        {
            get { return _key; }
            set
            {
                if ( value == _key )
                    return;
                _key = value;
                OnKeyChanged( );
            }
        }

        public string Note { get; set; }

        public SecureString MasterPassword { get; set; }

        public Guid? MasterPasswordId
        {
            get { return _passwordStore.IdentifyMasterPassword( MasterPassword ); }
        }

        public bool IsKeyStored
        {
            get { throw new NotImplementedException( ); }
        }

        public bool IsPasswordLoaded { get; private set; }

        public bool IsDirty { get; private set; }

        public IEnumerable<IPasswordGenerator> Generators { get; private set; }

        public IPasswordGenerator SelectedGenerator { get; set; }

        public string GeneratedPassword( IPasswordGenerator generator )
        {
            if ( string.IsNullOrWhiteSpace( Key ) )
                return string.Empty;
            if ( MasterPassword.Length == 0 )
                return string.Empty;
            return generator.MakePassword( Key, MasterPassword );
        }

        public void LoadPassword( )
        {
            throw new NotImplementedException( );
        }

        public void UnloadPassword( )
        {
            throw new NotImplementedException( );
        }

        public void SavePassword( )
        {
            if ( SelectedGenerator == null )
                return;
            string password = GeneratedPassword( SelectedGenerator );
            if ( string.IsNullOrEmpty( password ) )
                return;

            Guid masterPasswordId = MasterPasswordId ?? _newGuidFactory();
            _passwordStore.AddOrUpdate( _digester.Digest( Key, password, masterPasswordId, SelectedGenerator.Id, Note ));
        }

        public void DeletePassword( )
        {
            throw new NotImplementedException( );
        }

        // PRIVATE

        private void OnKeyChanged( )
        {
            IsDirty = true;
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordDigester _digester;
        private readonly Func<Guid> _newGuidFactory;
        private string _key;
    }
}