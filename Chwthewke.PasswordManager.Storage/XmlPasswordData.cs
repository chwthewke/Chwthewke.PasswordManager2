using System.Collections.Generic;

namespace Chwthewke.PasswordManager.Storage
{
    public class XmlPasswordData : IPasswordData
    {
        internal XmlPasswordData( PasswordSerializer2 serializer, IPasswordStore store )
        {
            _serializer = serializer;
            _store = store;
        }

        public IEnumerable<PasswordDigestDocument> LoadPasswords( )
        {
            return _serializer.Load( _store );
        }

        public void SavePasswords( IEnumerable<PasswordDigestDocument> passwords )
        {
            _serializer.Save( passwords, _store );
        }

        private readonly PasswordSerializer2 _serializer;
        private readonly IPasswordStore _store;
    }
}