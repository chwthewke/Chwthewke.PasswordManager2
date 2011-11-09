using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordExchange2 : IPasswordExchange
    {
        public PasswordExchange2( IPasswordRepository passwordRepository )
        {
            _passwordRepository = passwordRepository;
        }

        // TODO possibly return a "report" to be presented to the user
        public void ImportPasswords( FileInfo externalPasswordFile )
        {
            ExternalPasswordRepository( externalPasswordFile ).MergeInto( _passwordRepository );
        }

        public void ExportPasswords( FileInfo targetFile )
        {
            _passwordRepository.MergeInto( ExternalPasswordRepository( targetFile ) );
        }

        private static IPasswordRepository ExternalPasswordRepository( FileInfo targetFile )
        {
            return PasswordManagerStorage.CreateService( new FileTextResource( targetFile ) ).PasswordRepository;
        }

        private readonly IPasswordRepository _passwordRepository;
    }
}