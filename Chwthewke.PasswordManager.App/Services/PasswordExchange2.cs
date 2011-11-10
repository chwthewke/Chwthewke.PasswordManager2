using System.IO;
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
            _passwordRepository.Merge( ExternalPasswordRepository( externalPasswordFile ) );
        }

        public void ExportPasswords( FileInfo targetFile )
        {
            ExternalPasswordRepository( targetFile ).Merge( _passwordRepository );
        }

        private static IPasswordRepository ExternalPasswordRepository( FileInfo targetFile )
        {
            return PasswordManagerStorage.CreateService( new FileTextResource( targetFile ) ).PasswordRepository;
        }

        private readonly IPasswordRepository _passwordRepository;
    }
}