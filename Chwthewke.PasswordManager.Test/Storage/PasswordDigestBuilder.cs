using System;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class PasswordDigestBuilder
    {
        private string _key = "key";
        private byte[ ] _hash = new byte[ ] { };
        private Guid _masterPasswordId = default( Guid );
        private Guid _passwordGeneratorId = default( Guid );
        private DateTime _creationTime;
        private string _note = string.Empty;

        public PasswordDigestBuilder WithKey( string key )
        {
            _key = key;
            return this;
        }

        public PasswordDigestBuilder WithHash( byte[ ] hash )
        {
            _hash = hash;
            return this;
        }

        public PasswordDigestBuilder WithMasterPasswordId( Guid masterPasswordId )
        {
            _masterPasswordId = masterPasswordId;
            return this;
        }

        public PasswordDigestBuilder WithGeneratorId( Guid generatorId )
        {
            _passwordGeneratorId = generatorId;
            return this;
        }

        public PasswordDigestBuilder WithCreationTime( DateTime creationTime )
        {
            _creationTime = creationTime;
            return this;
        }

        public PasswordDigestBuilder WithNote( string note )
        {
            _note = note;
            return this;
        }

        public static implicit operator PasswordDigest( PasswordDigestBuilder builder )
        {
            return builder.Build( );
        }

        private PasswordDigest Build( )
        {
            return new PasswordDigest( _key, _hash, _masterPasswordId, _passwordGeneratorId, _creationTime, _note );
        }
    }
}