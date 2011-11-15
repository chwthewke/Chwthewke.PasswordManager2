using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordExchange2 : IPasswordExchange
    {

        public PasswordExchange2( IPasswordManagerStorage storage )
        {
            _storage = storage;
        }

        // TODO possibly return a "report" to be presented to the user
        public void ImportPasswords( FileInfo externalPasswordFile )
        {
            _storage.PasswordRepository.Merge( ExternalPasswordRepository( externalPasswordFile ) );
        }

        public void ExportPasswords( FileInfo targetFile )
        {
            ExternalPasswordRepository( targetFile ).Merge( _storage.PasswordRepository );
        }

        private static IPasswordRepository ExternalPasswordRepository( FileInfo targetFile )
        {
            return PasswordManagerStorage.CreateService( new FileTextResource( targetFile ) ).PasswordRepository;
        }

        private readonly IPasswordManagerStorage _storage;

    }
}