using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using Chwthewke.PasswordManager.Storage;
using System.Linq;

namespace Chwthewke.PasswordManager.Test.Storage
{
    public class TestPasswordStore : IPasswordStore
    {
        public PasswordInfo Add( PasswordInfo passwordInfo )
        {
            PasswordInfo previousPasswordInfo = FindPasswordInfo( passwordInfo.Key );

            if ( previousPasswordInfo != null )
                _passwords.Remove( previousPasswordInfo );

            _passwords.Add( passwordInfo );

            return previousPasswordInfo;
        }

        public bool Remove( PasswordInfo passwordInfo )
        {
            return _passwords.Remove( passwordInfo );
        }

        public IEnumerable<PasswordInfo> Passwords
        {
            get { return new ReadOnlyCollection<PasswordInfo>( _passwords ); }
        }

        public PasswordInfo FindPasswordInfo( string key )
        {
            return _passwords.FirstOrDefault( p => p.Key == key );
        }

        private readonly IList<PasswordInfo> _passwords = new List<PasswordInfo>( );
    }
}