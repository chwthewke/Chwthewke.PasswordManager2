﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    // TODO dirty checks, other logic may be better implemented with reference to "original" content (PasswordDigest ?)
    internal class PasswordEditorController : IPasswordEditorController
    {
        public PasswordEditorController( IPasswordDatabase passwordDatabase,
                                         IPasswordDigester digester,
                                         Func<Guid> newGuidFactory,
                                         IEnumerable<IPasswordGenerator> generators,
                                         IMasterPasswordMatcher masterPasswordMatcher,
                                         string password )
        {
            _passwordDatabase = passwordDatabase;
            _newGuidFactory = newGuidFactory;
            _masterPasswordMatcher = masterPasswordMatcher;
            _digester = digester;
            _key = string.Empty;
            _note = string.Empty;
            _masterPassword = new SecureString( );
            Generators = generators;

            if ( password != null )
                InitializeWith( password );
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
                UpdateDirtiness( );
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

        public SecureString MasterPassword
        {
            get { return _masterPassword; }
            set
            {
                _masterPassword = value;
                UpdateDirtiness( );
            }
        }

        public Guid? MasterPasswordId
        {
            get { return _masterPasswordMatcher.IdentifyMasterPassword( MasterPassword ); }
        }



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

        public Guid? ExpectedMasterPasswordId { get { return Baseline == null ? (Guid?)null : Baseline.MasterPasswordId; } }

        public bool IsPasswordLoaded { get { return Baseline != null; } }

        private DateTime? CreationTime { get { return Baseline == null ? (DateTime?)null : Baseline.CreationTime; } }

        private PasswordDigest Baseline { get; set; }


        public string GeneratedPassword( IPasswordGenerator generator )
        {
            if ( string.IsNullOrWhiteSpace( Key ) )
                return string.Empty;
            if ( MasterPassword.Length == 0 )
                return string.Empty;
            return generator.MakePassword( Key, MasterPassword );
        }


        public void SavePassword( )
        {
            if ( SelectedGenerator == null )
                return;
            string password = GeneratedPassword( SelectedGenerator );
            if ( string.IsNullOrEmpty( password ) )
                return;

            Guid masterPasswordId = MasterPasswordId ?? _newGuidFactory( );

            Baseline = _digester.Digest( Key, password, masterPasswordId, SelectedGenerator.Id,
                                                      CreationTime, Note );
            _passwordDatabase.AddOrUpdate( Baseline );

            IsDirty = false;
        }

        public void DeletePassword( )
        {
            if ( !IsPasswordLoaded )
                return;
            _passwordDatabase.Remove( Key );

            Baseline = null;

            IsDirty = true;
        }

        // PRIVATE

        private void InitializeWith( string passwordKey )
        {
            Baseline = _passwordDatabase.FindByKey( passwordKey );
            if ( Baseline == null )
                return;


            _key = Baseline.Key;
            _note = Baseline.Note;
            SelectedGenerator = GeneratorById( Baseline.PasswordGeneratorId );
            IsDirty = false;
        }

        private IPasswordGenerator GeneratorById( Guid passwordGeneratorId )
        {
            return Generators.FirstOrDefault( g => g.Id == passwordGeneratorId );
        }

        private PasswordDigest GetDigest( )
        {
            return _passwordDatabase.FindByKey( Key );
        }

        private void UpdateDirtiness( )
        {
            if ( !IsPasswordLoaded )
            {
                IsDirty = !( string.IsNullOrEmpty( Key ) && string.IsNullOrEmpty( Note ) && MasterPassword.Length == 0 );
            }
            else
            {
                PasswordDigest digest = GetDigest( );
                IsDirty = Note != digest.Note
                          || SelectedGenerator == null
                          || SelectedGenerator.Id != digest.PasswordGeneratorId;
            }
        }

        private readonly IPasswordDatabase _passwordDatabase;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IPasswordDigester _digester;
        private readonly Func<Guid> _newGuidFactory;

        private string _key = string.Empty;
        private string _note;
        private IPasswordGenerator _selectedGenerator;
        private SecureString _masterPassword;
    }
}