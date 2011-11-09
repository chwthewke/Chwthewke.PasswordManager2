using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    [ Obsolete ]
    public class PasswordEditorControllerFactory
    {
        public PasswordEditorControllerFactory( IPasswordDatabase passwordDatabase, IPasswordDigester passwordDigester, Func<Guid> newGuidFactory,
                                                IEnumerable<IPasswordGenerator> generators, IMasterPasswordMatcher masterPasswordMatcher )
        {
            _passwordDatabase = passwordDatabase;
            _passwordDigester = passwordDigester;
            _newGuidFactory = newGuidFactory;
            _generators = generators;
            _masterPasswordMatcher = masterPasswordMatcher;
        }

        public IPasswordEditorController PasswordEditorControllerFor( string passwordKey )
        {
            return new PasswordEditorController( _passwordDatabase, _passwordDigester,
                                                 _newGuidFactory, _generators,
                                                 _masterPasswordMatcher, passwordKey );
        }

        private readonly IPasswordDatabase _passwordDatabase;
        private readonly IPasswordDigester _passwordDigester;
        private readonly Func<Guid> _newGuidFactory;
        private readonly IEnumerable<IPasswordGenerator> _generators;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
    }
}