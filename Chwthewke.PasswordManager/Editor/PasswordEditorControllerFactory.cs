using System;
using System.Collections.Generic;
using Chwthewke.PasswordManager.Engine;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.Editor
{
    internal class PasswordEditorControllerFactory : IPasswordEditorControllerFactory
    {
        public PasswordEditorControllerFactory( IPasswordRepository passwordRepository,
                                                IPasswordDigester passwordDigester,
                                                Func<Guid> newGuidFactory,
                                                IEnumerable<IPasswordGenerator> generators )
        {
            _passwordRepository = passwordRepository;
            _passwordDigester = passwordDigester;
            _newGuidFactory = newGuidFactory;
            _generators = generators;
        }

        private readonly IPasswordRepository _passwordRepository;
        private readonly IPasswordDigester _passwordDigester;
        private readonly Func<Guid> _newGuidFactory;
        private readonly IEnumerable<IPasswordGenerator> _generators;


        public IPasswordEditorController CreatePasswordEditorController( )
        {
            return new PasswordEditorController( _passwordRepository, _passwordDigester, _newGuidFactory, _generators );
        }
    }
}