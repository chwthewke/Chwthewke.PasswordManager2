using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorControllerFactory : IPasswordEditorControllerFactory
    {
        public PasswordEditorControllerFactory( IPasswordStore passwordStore,
                                                IPasswordDigester passwordDigester,
                                                Func<Guid> newGuidFactory,
                                                IEnumerable<IPasswordGenerator> generators )
        {
            _passwordStore = passwordStore;
            _passwordDigester = passwordDigester;
            _newGuidFactory = newGuidFactory;
            _generators = generators;
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordDigester _passwordDigester;
        private readonly Func<Guid> _newGuidFactory;
        private readonly IEnumerable<IPasswordGenerator> _generators;


        public IPasswordEditorController CreatePasswordEditorController( )
        {
            return new PasswordEditorController( _passwordStore, _passwordDigester, _newGuidFactory, _generators );
        }
    }
}