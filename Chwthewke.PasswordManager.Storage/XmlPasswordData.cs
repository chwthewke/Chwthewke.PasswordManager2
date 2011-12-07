using System;
using System.Collections.Generic;
using System.Linq;

namespace Chwthewke.PasswordManager.Storage
{
    public class XmlPasswordData : IPasswordData
    {
        public static IPasswordData From( ITextResource textResource )
        {
            return new XmlPasswordData( new PasswordSerializer( ), textResource );
        }

        internal XmlPasswordData( PasswordSerializer serializer, ITextResource store )
        {
            if ( serializer == null )
                throw new ArgumentNullException( "serializer" );
            if ( store == null )
                throw new ArgumentNullException( "store" );
            _serializer = serializer;
            _store = store;
        }

        public IEnumerable<PasswordDigestDocument> LoadPasswords( )
        {
            return _serializer.Load( _store ).ToList( );
        }

        public void SavePasswords( IEnumerable<PasswordDigestDocument> passwords )
        {
            _serializer.Save( passwords, _store );
        }

        private readonly PasswordSerializer _serializer;
        private readonly ITextResource _store;
    }
}