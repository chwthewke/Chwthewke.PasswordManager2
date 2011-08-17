using System.Collections.Generic;
using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordImporter : IPasswordImporter
    {
        public PasswordImporter( IPasswordSerializer passwordSerializer, IPasswordDatabase passwordDatabase )
        {
            _passwordSerializer = passwordSerializer;
            _passwordDatabase = passwordDatabase;
        }

        // TODO possibly return a "report" to be presented to the user
        public void ImportPasswords( FileInfo externalPasswordFile )
        {
            IEnumerable<PasswordDigest> passwords = _passwordSerializer.Load( new FilePasswordStore( externalPasswordFile ) );


            foreach ( PasswordDigest passwordDigest in passwords )
            {
                if ( _passwordDatabase.FindByKey( passwordDigest.Key ) == null )
                    _passwordDatabase.AddOrUpdate( passwordDigest );
            }
        }

        private readonly IPasswordSerializer _passwordSerializer;
        private readonly IPasswordDatabase _passwordDatabase;
    }
}