using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
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
            }
        }

        public string Note
        {
            get { return _note; }
            set { _note = value; }
        }

        public SecureString MasterPassword
        {
            get { return _masterPassword; }
            set { _masterPassword = value; }
        }

        public Guid? MasterPasswordId
        {
            get { return _masterPasswordMatcher.IdentifyMasterPassword( MasterPassword ); }
        }

        public bool IsSaveable
        {
            get
            {
                if ( String.IsNullOrWhiteSpace( _key ) || MasterPassword.Length == 0 || SelectedGenerator == null )
                    return false;

                if ( Baseline == null )
                    return true;

                return !BaselineComparer.Equals( Baseline, MakeDigest( ) );
            }
        }

        public IEnumerable<IPasswordGenerator> Generators { get; private set; }

        public IPasswordGenerator SelectedGenerator
        {
            get { return _selectedGenerator; }
            set { _selectedGenerator = value; }
        }

        public Guid? ExpectedMasterPasswordId
        {
            get { return Baseline == null ? (Guid?) null : Baseline.MasterPasswordId; }
        }

        public bool IsPasswordLoaded
        {
            get { return Baseline != null; }
        }

        private DateTime? CreationTime
        {
            get { return Baseline == null ? (DateTime?) null : Baseline.CreationTime; }
        }

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
            if ( !IsSaveable )
                return;

            Baseline = MakeDigest( );
            _passwordDatabase.AddOrUpdate( Baseline );
        }


        public void DeletePassword( )
        {
            if ( !IsPasswordLoaded )
                return;
            _passwordDatabase.Remove( Key );

            Baseline = null;
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
        }

        private PasswordDigest MakeDigest( )
        {
            string password = GeneratedPassword( SelectedGenerator );

            Guid masterPasswordId = MasterPasswordId ?? _newGuidFactory( );

            PasswordDigest newDigest = _digester.Digest( Key, password, masterPasswordId, SelectedGenerator.Id,
                                                         CreationTime, Note );
            return newDigest;
        }

        private IPasswordGenerator GeneratorById( Guid passwordGeneratorId )
        {
            return Generators.FirstOrDefault( g => g.Id == passwordGeneratorId );
        }

        private readonly IPasswordDatabase _passwordDatabase;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IPasswordDigester _digester;
        private readonly Func<Guid> _newGuidFactory;

        private string _key = string.Empty;
        private string _note;
        private IPasswordGenerator _selectedGenerator;
        private SecureString _masterPassword;

        private static readonly IEqualityComparer<PasswordDigest> BaselineComparer =
            new BaselineDigestComparator( );

        private class BaselineDigestComparator : IEqualityComparer<PasswordDigest>
        {
            public bool Equals( PasswordDigest x, PasswordDigest y )
            {
                if ( x == null )
                    return y == null;
                if ( y == null )
                    return false;

                if ( y.Note != x.Note )
                    return false;
                if ( y.MasterPasswordId != x.MasterPasswordId )
                    return false;
                if ( y.PasswordGeneratorId != x.PasswordGeneratorId )
                    return false;
                if ( !Equals( y.Hash, x.Hash ) )
                    return false;
                return true;
            }

            public int GetHashCode( PasswordDigest obj )
            {
                throw new NotImplementedException( );
            }
        }
    }
}