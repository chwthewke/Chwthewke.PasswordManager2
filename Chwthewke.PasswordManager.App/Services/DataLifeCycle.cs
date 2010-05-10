using System.IO;
using Chwthewke.PasswordManager.App.Properties;
using Chwthewke.PasswordManager.Storage;

namespace Chwthewke.PasswordManager.App.Services
{
    public class DataLifeCycle
    {
        public DataLifeCycle( IPasswordStore passwordStore, IPasswordStoreSerializer serializer )
        {
            _passwordStore = passwordStore;
            _serializer = serializer;
        }

        public void Load( )
        {
            string serializedPasswordStore = Settings.Default.PasswordStore;
            _serializer.Load( _passwordStore, new StringReader( serializedPasswordStore ) );
        }

        public void Save( )
        {
            
        }

        private readonly IPasswordStore _passwordStore;
        private readonly IPasswordStoreSerializer _serializer;
    }
}