using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class DataLifeCycle
    {
        public void Load( )
        {
            string serializedPasswordStore = Settings.Default.PasswordStore;
            _serializer.Load( _passwordStore, new MemoryStream( ) );
        }

        public void Save( )
        {
            
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordStoreSerializer _serializer;
    }
}