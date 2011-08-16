﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Security;
using System.Text;
using Chwthewke.PasswordManager.Engine;

namespace Chwthewke.PasswordManager.Storage
{
    class MasterPasswordMatcher : IMasterPasswordMatcher
    {
        public MasterPasswordMatcher( IEnumerable<IPasswordGenerator> generators, IHashFactory hashFactory, IPasswordRepository repository )
        {
            _generators = generators;
            _hashFactory = hashFactory;
            _repository = repository;
        }

        public Guid? IdentifyMasterPassword( SecureString masterPassword )
        {
            IEnumerable<PasswordDigest> candidates =
                _repository.Passwords.GroupBy( pw => pw.MasterPasswordId ).Select( g => g.First( ) );

            PasswordDigest matchingDigest =
                candidates.FirstOrDefault( dig => MatchMasterPassword( dig, masterPassword ) );


            return matchingDigest != null ? matchingDigest.MasterPasswordId : ( Guid? ) null;
        }

        private bool MatchMasterPassword( PasswordDigest dig, SecureString masterPassword )
        {
            IPasswordGenerator generator = _generators.FirstOrDefault( g => g.Id == dig.PasswordGeneratorId );
            if ( generator == null )
                return false;
            string generatedPassword = generator.MakePassword( dig.Key, masterPassword );
            return _hashFactory.GetHash( )
                .Append( PasswordDigester.DigestSalt, Encoding.UTF8 )
                .Append( generatedPassword, Encoding.UTF8 )
                .GetValue( )
                .SequenceEqual( dig.Hash );
        }

        private readonly IEnumerable<IPasswordGenerator> _generators;
        private readonly IHashFactory _hashFactory;
        private readonly IPasswordRepository _repository;


    }
}