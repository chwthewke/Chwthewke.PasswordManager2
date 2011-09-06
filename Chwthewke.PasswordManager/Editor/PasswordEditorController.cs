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
            Note = string.Empty;
            MasterPassword = new SecureString( );
            Generators = generators;

            _iterationsByGenerator = Generators.ToDictionary( g => g, g => 0 );

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

        public string Note { get; set; }

        public SecureString MasterPassword { get; set; }

        public Guid? MasterPasswordId
        {
            get { return _masterPasswordMatcher.IdentifyMasterPassword( MasterPassword ); }
        }

        public IPasswordGenerator SelectedGenerator { get; set; }

        public string SelectedPassword
        {
            get
            {
                if ( SelectedGenerator == null )
                    return string.Empty;
                return GeneratedPassword( SelectedGenerator );
            }
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

        public Guid? ExpectedMasterPasswordId
        {
            get { return Baseline == null ? (Guid?) null : Baseline.MasterPasswordId; }
        }

        public bool IsPasswordLoaded
        {
            get { return Baseline != null; }
        }

        public void IncreaseIteration( IPasswordGenerator generator )
        {
            _iterationsByGenerator[ generator ]++;
        }

        public void DecreaseIteration( IPasswordGenerator generator )
        {
            if ( _iterationsByGenerator[ generator ] <= 0 )
                return;
            _iterationsByGenerator[ generator ]--;
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
            return generator.MakePasswords( Key, MasterPassword ).ElementAt( _iterationsByGenerator[ generator ] );
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

        public void ReloadBaseline( )
        {
            _passwordDatabase.Reload( );
            Baseline = _passwordDatabase.FindByKey( Key );
        }

        // PRIVATE

        private void InitializeWith( string passwordKey )
        {
            Baseline = _passwordDatabase.FindByKey( passwordKey );
            if ( Baseline == null )
                return;


            _key = Baseline.Key;
            Note = Baseline.Note;
            SelectedGenerator = GeneratorById( Baseline.PasswordGeneratorId );
        }

        private PasswordDigest MakeDigest( )
        {
            string password = GeneratedPassword( SelectedGenerator );

            Guid masterPasswordId = MasterPasswordId.HasValue ? MasterPasswordId.Value : _newGuidFactory( );

            PasswordDigest newDigest = _digester.Digest( Key, password, masterPasswordId, SelectedGenerator.Id,
                                                         CreationTime, _iterationsByGenerator[ SelectedGenerator ], Note );
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

        private readonly IDictionary<IPasswordGenerator, int> _iterationsByGenerator;

        private string _key = string.Empty;

        private static readonly IEqualityComparer<PasswordDigest> BaselineComparer =
            new BaselineDigestComparator( );

        private class BaselineDigestComparator : IEqualityComparer<PasswordDigest>
        {
            public bool Equals( PasswordDigest x, PasswordDigest y )
            {
                if ( ReferenceEquals( x, y ) )
                    return true;

                if ( ReferenceEquals( x, null ) || ReferenceEquals( y, null ) )
                    return false;

                if ( y.Note != x.Note )
                    return false;
                if ( y.MasterPasswordId != x.MasterPasswordId )
                    return false;
                if ( y.PasswordGeneratorId != x.PasswordGeneratorId )
                    return false;
                if ( !y.Hash.SequenceEqual( x.Hash ) )
                    return false;
                return true;
            }

            public int GetHashCode( PasswordDigest obj )
            {
                if ( obj == null )
                    return 0;
                int result = ( obj.Note == null ? 0 : obj.Note.GetHashCode( ) );
                result = 397 * result ^ ( obj.MasterPasswordId.GetHashCode( ) );
                result = 397 * result ^ ( obj.PasswordGeneratorId.GetHashCode( ) );
                return result;
            }
        }
    }
}