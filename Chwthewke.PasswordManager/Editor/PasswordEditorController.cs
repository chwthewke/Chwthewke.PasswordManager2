﻿using System;
using System.Collections.Generic;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorController : IPasswordEditorController
    {
        public PasswordEditorController( IPasswordStore passwordStore,
                                         IPasswordDigester digester,
                                         Func<Guid> newGuidFactory,
                                         IEnumerable<IPasswordGenerator> generators )
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
                if ( IsPasswordLoaded )
                    return;
                _key = value;
                IsDirty = true;
            }
        }

        public string Note
        {
            get { return _note; }
            set
            {
                _note = value;
                UpdateDirtiness( );
            }
        }

        public SecureString MasterPassword { get; set; }

        public Guid? MasterPasswordId
        {
            get { return _passwordStore.IdentifyMasterPassword( MasterPassword ); }
        }

        public Guid? ExpectedMasterPasswordId { get; private set; }

        public bool IsKeyStored
        {
            get { return GetDigest( ) != null; }
        }

        public bool IsPasswordLoaded { get; private set; }

        public bool IsDirty { get; private set; }

        public IEnumerable<IPasswordGenerator> Generators { get; private set; }

        public IPasswordGenerator SelectedGenerator
        {
            get { return _selectedGenerator; }
            set
            {
                _selectedGenerator = value;
                UpdateDirtiness( );
            }
        }

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
            if ( IsPasswordLoaded )
                return;

            PasswordDigest digest = GetDigest( );
            if ( digest == null )
                return;

            Note = digest.Note;
            ExpectedMasterPasswordId = digest.MasterPasswordId;
            SelectedGenerator = GeneratorById( digest.PasswordGeneratorId );
            IsDirty = false;
            IsPasswordLoaded = true;
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

            Guid masterPasswordId = MasterPasswordId ?? _newGuidFactory( );
            _passwordStore.AddOrUpdate( _digester.Digest( Key, password, masterPasswordId, SelectedGenerator.Id, Note ) );
        }

        public void DeletePassword( )
        {
            throw new NotImplementedException( );
        }

        // PRIVATE

        private IPasswordGenerator GeneratorById( Guid passwordGeneratorId )
        {
            return Generators.FirstOrDefault( g => g.Id == passwordGeneratorId );
        }

        private PasswordDigest GetDigest( )
        {
            return _passwordStore.FindPasswordInfo( Key );
        }

        private void UpdateDirtiness( )
        {
            if ( !IsPasswordLoaded )
                return;
            PasswordDigest digest = GetDigest( );
            IsDirty = Note != digest.Note
                      || SelectedGenerator.Id != digest.PasswordGeneratorId;
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordDigester _digester;
        private readonly Func<Guid> _newGuidFactory;

        private string _key;
        private string _note;
        private IPasswordGenerator _selectedGenerator;
    }
}