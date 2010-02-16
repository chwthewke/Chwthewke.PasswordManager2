using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage.Serialization
{
    public interface IExportablePasswordCollection
    {
        IEnumerable<PasswordDTO> ExportPasswords( );
        void ImportPasswords( IEnumerable<PasswordDTO> passwordDtos );
    }
}