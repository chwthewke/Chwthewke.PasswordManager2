using System.IO;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class PasswordExchange : IPasswordExchange
    {

        public PasswordExchange( IPasswordManagerStorage storage )
        {
            _storage = storage;
        }

        public void ImportPasswords( FileInfo externalPasswordFile )
        {
            var importedPasswords =
                XmlPasswordData.From( new FileTextResource( externalPasswordFile ) ).LoadPasswords( );
            _storage.PasswordRepository.Merge( importedPasswords );
        }

        public void ExportPasswords( FileInfo targetFile )
        {
            ExternalPasswordRepository( targetFile ).Merge( _storage.PasswordRepository.PasswordData.LoadPasswords( ) );
        }

        private static IPasswordRepository ExternalPasswordRepository( FileInfo targetFile )
        {
            return PasswordManagerStorage.CreateService( new FileTextResource( targetFile ) ).PasswordRepository;
        }

        private readonly IPasswordManagerStorage _storage;

    }
}