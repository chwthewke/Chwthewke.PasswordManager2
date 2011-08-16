using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{

    // TODO replace with factory delegate
    internal class PasswordEditorControllerFactory : IPasswordEditorControllerFactory
    {
        public PasswordEditorControllerFactory( IPasswordRepository passwordRepository,
                                                IPasswordDatabase passwordDatabase,
                                                IMasterPasswordMatcher masterPasswordMatcher,
                                                IPasswordDigester passwordDigester,
                                                Func<Guid> newGuidFactory,
                                                IEnumerable<IPasswordGenerator> generators )
        {
            _passwordRepository = passwordRepository;
            _passwordDatabase = passwordDatabase;
            _masterPasswordMatcher = masterPasswordMatcher;
            _passwordDigester = passwordDigester;
            _newGuidFactory = newGuidFactory;
            _generators = generators;
        }

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordDatabase _passwordDatabase;
        private readonly IMasterPasswordMatcher _masterPasswordMatcher;
        private readonly IPasswordDigester _passwordDigester;
        private readonly Func<Guid> _newGuidFactory;
        private readonly IEnumerable<IPasswordGenerator> _generators;


        public IPasswordEditorController CreatePasswordEditorController( )
        {
            return new PasswordEditorController( _passwordRepository, _passwordDatabase, _passwordDigester, _newGuidFactory, _generators, _masterPasswordMatcher );
        }
    }
}