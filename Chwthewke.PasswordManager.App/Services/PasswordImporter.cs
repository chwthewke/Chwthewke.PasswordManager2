using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordImporter : IPasswordImporter
    {
        public PasswordImporter( IPasswordSerializer passwordSerializer, IPasswordRepository passwordRepository )
        {
            _passwordSerializer = passwordSerializer;
            _passwordRepository = passwordRepository;
        }

        // TODO possibly return a "report" to be presented to the user
        public void ImportPasswords( FileInfo externalPasswordFile )
        {
            IPasswordStore passwordStore = new ExternalPasswordStore( _passwordSerializer, externalPasswordFile );
            IEnumerable<PasswordDigest> passwords = passwordStore.Load( );

            foreach ( PasswordDigest passwordDigest in passwords )
            {
                if ( _passwordRepository.FindPasswordInfo( passwordDigest.Key ) == null )
                    _passwordRepository.AddOrUpdate( passwordDigest );
            }
        }

        private readonly IPasswordSerializer _passwordSerializer;
        private readonly IPasswordRepository _passwordRepository;
    }
}